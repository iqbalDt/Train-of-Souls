using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance;

    [Header("Current NPC")]
    public NPC_Controller currentNPC;

    [Header("Managers")]
    public TelephoneManager telephoneManager;
    public WinLoseManager winLoseManager; // ❌ JANGAN DIUBAH
    public TicketPrinter ticketPrinter;

    [Header("UI")]
    public Button telephoneButton;
    public LieDetectorUI lieDetectorUI;
    public Button lieDetectorButton;

    // ===== INTERNAL STATE =====
    private bool dialogFinishedThisNPC;
    private bool telephoneUsedThisNPC;
    private bool waitingForTicket;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var spawner = FindFirstObjectByType<NPC_Spawner>();
        spawner?.SpawnNextNPC();

        SetTelephoneInteractable(false);
        SetLieDetectorInteractable(false);
    }

    // =========================
    // NPC FLOW
    // =========================

    // === VERSI BARU (DIREKOMENDASIKAN) ===
    public void OnNPCReachedMiddle(NPC_Controller npc)
    {
        currentNPC = npc;
        ResetNPCState();
    }

    // === BACKWARD COMPATIBILITY (FIX ERROR CS7036) ===
    public void OnNPCReachedMiddle()
    {
        var npc = FindFirstObjectByType<NPC_Controller>();
        if (npc != null)
            OnNPCReachedMiddle(npc);
    }

    void ResetNPCState()
    {
        dialogFinishedThisNPC = false;
        telephoneUsedThisNPC = false;
        waitingForTicket = false;

        SetTelephoneInteractable(false);
        SetLieDetectorInteractable(false);

        telephoneManager?.ForceClose();
        lieDetectorUI?.ShowNeutral();
        ticketPrinter?.ResetPrinter();
    }

    public void OnDialogFinished()
    {
        dialogFinishedThisNPC = true;
        SetTelephoneInteractable(!telephoneUsedThisNPC);
        SetLieDetectorInteractable(true);
    }

    // =========================
    // TELEPHONE
    // =========================

    public void OnTelephonePressed()
    {
        if (currentNPC == null) return;
        if (!dialogFinishedThisNPC) return;
        if (telephoneUsedThisNPC) return;
        if (waitingForTicket) return;

        telephoneUsedThisNPC = true;
        SetTelephoneInteractable(false);

        telephoneManager.StartTelephone(currentNPC.gameObject);
    }

    // =========================
    // LIE DETECTOR
    // =========================

    public void OnPressLieDetector()
    {
        if (currentNPC == null) return;
        if (!dialogFinishedThisNPC) return;
        if (waitingForTicket) return;

        var dialog = currentNPC.GetComponent<DialogBubbleSpawner_Gameplay>();
        if (dialog == null) return;

        var topic = dialog.GetActiveTopic();
        if (topic == null) return;

        var state = dialog.GetNPCState();
        var anim = currentNPC.GetComponent<NPC_AnimatorController>();

        // === UI INDICATOR ===
        if (lieDetectorUI != null)
        {
            switch (state)
            {
                case NPCState.Truth:
                    lieDetectorUI.ShowTruth();
                    break;
                case NPCState.Lie:
                    lieDetectorUI.ShowLie();
                    break;
                default:
                    lieDetectorUI.ShowNeutral();
                    break;
            }
        }

        // === NPC REACTION ===
        if (state == NPCState.Lie)
        {
            // 🔥 AUTO MAD
            if (anim != null)
                anim.ForceMadAndSpeak();

            dialog.ShowDetectorReaction(topic.reactionLie);
        }
        else if (state == NPCState.Truth)
        {
            dialog.ShowDetectorReaction(topic.reactionTruth);
        }
        else
        {
            dialog.ShowDetectorReaction(topic.reactionNeutral);
        }
    }

    // =========================
    // PLAYER DECISION
    // =========================

    public void OnChooseHeaven()
    {
        if (currentNPC == null) return;
        if (waitingForTicket) return;

        waitingForTicket = true;

        SetTelephoneInteractable(false);
        SetLieDetectorInteractable(false);
        telephoneManager?.ForceClose();

        ticketPrinter?.PrintHeavenTicket();
    }

    public void OnChooseHell()
    {
        if (currentNPC == null) return;
        if (waitingForTicket) return;

        waitingForTicket = true;

        SetTelephoneInteractable(false);
        SetLieDetectorInteractable(false);
        telephoneManager?.ForceClose();

        ticketPrinter?.PrintHellTicket();
    }

    // =========================
    // TICKET CALLBACK
    // =========================

    public void OnTicketTaken()
    {
        if (currentNPC == null) return;
        StartCoroutine(HandleNPCExitAndNext());
    }

    IEnumerator HandleNPCExitAndNext()
    {
        var spawner = FindFirstObjectByType<NPC_Spawner>();
        if (spawner == null) yield break;

        currentNPC.StartExitMovement();
        yield return new WaitForSeconds(1.2f);

        currentNPC = null;
        waitingForTicket = false;

        spawner.SpawnNextNPC();
    }

    // =========================
    // UI UTIL
    // =========================

    void SetTelephoneInteractable(bool value)
    {
        if (telephoneButton != null)
            telephoneButton.interactable = value;
    }

    void SetLieDetectorInteractable(bool value)
    {
        if (lieDetectorButton != null)
            lieDetectorButton.interactable = value;
    }
}
