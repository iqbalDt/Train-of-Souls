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

    [Header("Train Bell Audio")]
    public AudioSource bellSource;
    public AudioClip trainBellClip;

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

    public void OnNPCReachedMiddle(NPC_Controller npc)
    {
        currentNPC = npc;
        ResetNPCState();
    }

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

        // ✅ RESET ANIMATOR NPC (FIX HALUS TAPI PENTING)
        if (currentNPC != null)
        {
            var anim = currentNPC.GetComponent<NPC_AnimatorController>();
            if (anim != null)
                anim.ResetAnimator();
        }
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

        if (state == NPCState.Lie)
        {
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

        // 🚶 NPC keluar
        currentNPC.StartExitMovement();
        yield return new WaitForSeconds(1.2f);

        // 🔔 Bel kereta
        if (bellSource != null && trainBellClip != null)
        {
            bellSource.PlayOneShot(trainBellClip);
            yield return new WaitForSeconds(trainBellClip.length);
        }

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
