using UnityEngine;

public class NPC_Spawner : MonoBehaviour
{
    public GameObject[] npcPrefabs;

    public RectTransform pointLeft;
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    public GameObject currentNPC;

    public void SpawnNextNPC()
    {
        if (currentNPC != null)
            Destroy(currentNPC);

        int prefabIndex = Random.Range(0, npcPrefabs.Length);

        GameObject npc = Instantiate(npcPrefabs[prefabIndex], transform);
        currentNPC = npc;

        RectTransform rt = npc.GetComponent<RectTransform>();
        rt.anchoredPosition = pointLeft.anchoredPosition;

        NPC_Controller ctrl = npc.GetComponent<NPC_Controller>();
        ctrl.pointMiddle = pointMiddle;
        ctrl.pointRight = pointRight;

        NPCState chosenState = (NPCState)Random.Range(0, 3);
        ctrl.npcState = chosenState;

        // Assign TOPIK dari NPC itu
        var dialog = npc.GetComponent<DialogBubbleSpawner_Gameplay>();
        dialog.AssignTopic(chosenState);

        // reset lie detector
        var lieUI = FindFirstObjectByType<LieDetectorUI>();
        lieUI?.ShowNeutral();
    }
}
