using UnityEngine;

public class NPC_Spawner : MonoBehaviour
{
    [Header("NPC Prefabs (isi 6 NPC)")]
    public GameObject[] npcPrefabs;

    [Header("Spawn Points")]
    public RectTransform pointLeft;
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    [Header("Current NPC")]
    public GameObject currentNPC;

    public void SpawnNextNPC()
    {
        // Pastikan clean
        if (currentNPC != null)
            Destroy(currentNPC);

        // Pilih NPC random
        int index = Random.Range(0, npcPrefabs.Length);
        GameObject npcObj = Instantiate(npcPrefabs[index], transform);
        currentNPC = npcObj;

        RectTransform rt = npcObj.GetComponent<RectTransform>();
        rt.anchoredPosition = pointLeft.anchoredPosition;

        NPC_Controller ctrl = npcObj.GetComponent<NPC_Controller>();
        ctrl.pointMiddle = pointMiddle;
        ctrl.pointRight = pointRight;

        // RANDOM STATE
        NPCState state = (NPCState)Random.Range(0, 3);
        ctrl.npcState = state;

        // PASS STATE KE DIALOG SYSTEM
        npcObj.GetComponent<DialogBubbleSpawner_Gameplay>().SetupDialog(state);

        // LIE DETECTOR HARUS NETRAL SAAT NPC MASUK
        var lieUI = FindFirstObjectByType<LieDetectorUI>();
        if (lieUI != null)
            lieUI.ShowNeutral();

        Debug.Log($"Spawn NPC: {npcObj.name}, State = {state}");
    }
}
