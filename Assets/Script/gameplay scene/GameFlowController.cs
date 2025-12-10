using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    [Header("References")]
    public NPC_Spawner spawner;

    [Header("Buttons")]
    public Button surgaButton;
    public Button nerakaButton;

    [Header("SFX")]
    public AudioSource trainAudio;

    [Header("NPC Count")]
    public int maxNPC = 5;
    private int spawnedCount = 0;

    void Start()
    {
        if (spawner == null)
            spawner = FindFirstObjectByType<NPC_Spawner>();

        spawnedCount = 0;
        SpawnFirstNPC();
    }

    void SpawnFirstNPC()
    {
        spawner.SpawnNextNPC();
        spawnedCount++;
    }

    public void OnNPCReachedMiddle()
    {
        // Buttons selalu aktif sesuai request sebelumnya
        surgaButton.gameObject.SetActive(true);
        nerakaButton.gameObject.SetActive(true);
    }

    public void OnDialogFinished()
    {
        // Lie detector bergerak setelah dialog selesai
        if (spawner != null && spawner.currentNPC != null)
        {
            NPC_Controller ctrl = spawner.currentNPC.GetComponent<NPC_Controller>();
            UpdateLieDetector(ctrl.npcState);
        }
    }

    // --- Mengatur Lie Detector ---
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

    // --- Handle pilihan pemain ---
    public void OnChooseHeaven()
    {
        HandleChoice(true);
    }

    public void OnChooseHell()
    {
        HandleChoice(false);
    }

    void HandleChoice(bool choseHeaven)
    {
        if (spawner.currentNPC != null)
        {
            NPC_Controller ctrl = spawner.currentNPC.GetComponent<NPC_Controller>();
            var dialog = spawner.currentNPC.GetComponent<DialogBubbleSpawner_Gameplay>();

            // Ambil moral value dari dialog
            var moralValue = dialog.GetCurrentMoralValue();

            // Cek apakah player benar atau salah
            bool correct = IsCorrectChoice(moralValue, choseHeaven);

            // Laporkan ke WinLoseManager
            WinLoseManager.Instance.RegisterChoice(correct);

            // NPC keluar frame
            ctrl.StartExitMovement();
        }

        StartCoroutine(SpawnNextAfterTrain());
    }

    // --- RULE BENAR / SALAH BERDASARKAN MORAL VALUE DIALOG ---
    bool IsCorrectChoice(DialogBubbleSpawner_Gameplay.MoralValue value, bool choseHeaven)
    {
        if (value == DialogBubbleSpawner_Gameplay.MoralValue.Heaven)
            return choseHeaven == true;

        if (value == DialogBubbleSpawner_Gameplay.MoralValue.Hell)
            return choseHeaven == false;

        return true;   // Neutral tidak pernah salah
    }

    IEnumerator SpawnNextAfterTrain()
    {
        if (trainAudio != null)
        {
            trainAudio.Stop();
            trainAudio.Play();
            yield return new WaitForSeconds(trainAudio.clip.length);
        }

        // Tidak spawn baru jika WinLoseManager akan memproses ending
        if (spawnedCount < maxNPC)
        {
            spawner.SpawnNextNPC();
            spawnedCount++;
        }
    }
}
