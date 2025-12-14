using UnityEngine;

public class NPC_Spawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs;

    [Header("Movement Points")]
    public RectTransform pointLeft;
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    [Header("Spawn Limit")]
    public int maxNPCPerDay = 5;

    [HideInInspector] public GameObject currentNPC;

    private int spawnedCount = 0;

    // =========================
    // SPAWN
    // =========================
    public void SpawnNextNPC()
    {
        if (spawnedCount >= maxNPCPerDay)
        {
            Debug.Log("NPC_Spawner: Max NPC reached, stop spawning.");
            return;
        }

        if (currentNPC != null)
            Destroy(currentNPC);

        int prefabIndex = Random.Range(0, npcPrefabs.Length);
        GameObject npc = Instantiate(npcPrefabs[prefabIndex], transform);
        currentNPC = npc;

        spawnedCount++;

        RectTransform rt = npc.GetComponent<RectTransform>();
        rt.anchoredPosition = pointLeft.anchoredPosition;

        NPC_Controller ctrl = npc.GetComponent<NPC_Controller>();
        ctrl.pointMiddle = pointMiddle;
        ctrl.pointRight = pointRight;

        NPCState chosenState = (NPCState)Random.Range(0, 3);
        ctrl.npcState = chosenState;

        var dialog = npc.GetComponent<DialogBubbleSpawner_Gameplay>();
        dialog.AssignTopic(chosenState);

        var lieUI = FindFirstObjectByType<LieDetectorUI>();
        lieUI?.ShowNeutral();

        Debug.Log($"NPC Spawned: {spawnedCount}/{maxNPCPerDay}");
    }

    // =========================
    // QUERY STATUS
    // =========================
    public bool HasReachedLimit()
    {
        return spawnedCount >= maxNPCPerDay;
    }

    // =========================
    // RESET
    // =========================
    public void ResetSpawner()
    {
        spawnedCount = 0;

        if (currentNPC != null)
            Destroy(currentNPC);

        currentNPC = null;
    }
}