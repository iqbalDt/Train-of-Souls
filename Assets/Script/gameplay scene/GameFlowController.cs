using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    [Header("References")]
    public NPC_Spawner spawner;

    [Header("UI Buttons")]
    public Button surgaButton;
    public Button nerakaButton;
    public Button telephoneButton;

    [Header("Audio")]
    public AudioSource trainAudio;

    [Header("Game Settings")]
    public int maxNPC = 5;
    private int spawnedCount = 0;

    void Start()
    {
        spawnedCount = 0;
        SpawnNPC();
    }

    void SpawnNPC()
    {
        // Reset telepon untuk NPC baru
        TelephoneManager.Instance.ResetTelephoneForNewNPC();

        // Spawn NPC baru
        spawner.SpawnNextNPC();
        spawnedCount++;

        // Enable tombol telepon untuk NPC ini
        if (telephoneButton != null)
            telephoneButton.interactable = true;

        // Pastikan tombol pilihan aktif
        surgaButton.interactable = true;
        nerakaButton.interactable = true;
    }

    // Dipanggil saat NPC mencapai titik tengah
    public void OnNPCReachedMiddle()
    {
        surgaButton.gameObject.SetActive(true);
        nerakaButton.gameObject.SetActive(true);
        telephoneButton.gameObject.SetActive(true);
    }

    // Dipanggil setelah dialog NPC selesai
    public void OnDialogFinished()
    {
        var npc = spawner.currentNPC;
        if (npc == null) return;

        var ctrl = npc.GetComponent<NPC_Controller>();
        UpdateLieDetector(ctrl.npcState);
    }

    // Update UI Lie Detector berdasarkan state NPC
    public void UpdateLieDetector(NPCState state)
    {
        var lieUI = FindFirstObjectByType<LieDetectorUI>();
        if (lieUI == null) return;

        if (state == NPCState.Lie)
            lieUI.ShowLie();
        else if (state == NPCState.Truth)
            lieUI.ShowTruth();
        else
            lieUI.ShowNeutral();
    }

    // ============================
    //   PLAYER CHOICE HANDLING
    // ============================

    public void OnChooseHeaven() => HandleChoice(true);
    public void OnChooseHell() => HandleChoice(false);

    void HandleChoice(bool choseHeaven)
    {
        if (spawner == null || spawner.currentNPC == null)
        {
            Debug.LogError("Tidak ada NPC aktif saat pilihan dilakukan.");
            return;
        }

        var npc = spawner.currentNPC;
        var dialog = npc.GetComponent<DialogBubbleSpawner_Gameplay>();

        if (dialog == null)
        {
            Debug.LogError("NPC tidak memiliki DialogBubbleSpawner_Gameplay!");
            return;
        }

        if (dialog.GetActiveTopic() == null)
        {
            Debug.LogError("NPC tidak punya activeTopic! Pastikan topics terisi.");
            return;
        }

        // Ambil moral value dari dialog yang sudah selesai
        var moralValue = dialog.GetCurrentMoralValue();
        bool correct = IsCorrectChoice(moralValue, choseHeaven);

        // Laporkan ke WinLoseManager
        WinLoseManager.Instance.RegisterChoice(correct);

        // NPC keluar area
        npc.GetComponent<NPC_Controller>().StartExitMovement();

        // Disable tombol pilihan sementara
        surgaButton.interactable = false;
        nerakaButton.interactable = false;

        StartCoroutine(SpawnDelay());
    }

    bool IsCorrectChoice(DialogBubbleSpawner_Gameplay.MoralValue value, bool choseHeaven)
    {
        if (value == DialogBubbleSpawner_Gameplay.MoralValue.Heaven)
            return choseHeaven;

        if (value == DialogBubbleSpawner_Gameplay.MoralValue.Hell)
            return !choseHeaven;

        return true; // neutral -> tidak pernah salah
    }

    IEnumerator SpawnDelay()
    {
        if (trainAudio != null)
        {
            trainAudio.Stop();
            trainAudio.Play();
            yield return new WaitForSeconds(trainAudio.clip.length);
        }

        if (spawnedCount < maxNPC)
            SpawnNPC();
    }

    // ============================
    //       TELEPHONE WRAPPER
    // ============================

    public void OnTelephonePressed()
    {
        if (spawner.currentNPC == null) return;

        TelephoneManager.Instance.CallRelative(spawner.currentNPC);
    }
}
