using UnityEngine;
using System.Collections;

public class GameFlowController : MonoBehaviour
{
    public NPC_Spawner spawner;
    public AudioSource trainDepartureSFX;

    public int maxNPC = 5;
    private int spawnedCount = 0;

    private bool waitingForNext = false;

    void Start()
    {
        StartNextNPC();
    }

    public void StartNextNPC()
    {
        if (spawnedCount >= maxNPC)
        {
            Debug.Log("Semua NPC selesai.");
            return;
        }

        spawner.SpawnNPC();
        spawnedCount++;

        waitingForNext = false;
    }

    public void ChooseSurga()
    {
        if (waitingForNext) return;

        waitingForNext = true;
        Debug.Log("PLAYER PILIH SURGA");

        HandleChoice();
    }

    public void ChooseNeraka()
    {
        if (waitingForNext) return;

        waitingForNext = true;
        Debug.Log("PLAYER PILIH NERAKA");

        HandleChoice();
    }

    void HandleChoice()
    {
        if (trainDepartureSFX != null)
            trainDepartureSFX.Play();

        if (spawner.currentNPC != null)
            spawner.currentNPC.GetComponent<NPC_Controller>().OnDialogFinished();

        StartCoroutine(DelayNextNPC());
    }

    IEnumerator DelayNextNPC()
    {
        // otomatis menunggu durasi audio kereta
        float waitTime = trainDepartureSFX.clip.length;

        yield return new WaitForSeconds(waitTime);

        StartNextNPC();
    }
}
