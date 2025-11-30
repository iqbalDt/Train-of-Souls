using UnityEngine;
using System.Collections.Generic;

public class NPC_Spawner : MonoBehaviour
{
    public GameObject npcPrefab;

    public RectTransform pointLeft;
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    public List<string[]> dialogSets = new List<string[]>
    {
        new string[]
        {
            "Aku mati karena tabrak lari… begitu saja.",
            "Aku memang mencuri… tapi bukan untuk diriku sendiri.",
            "Aku ambil dari orang kaya yang tamak.",
            "Dan kuberikan ke orang-orang yang kelaparan.",
            "Apakah itu membuatku jahat?"
        },

        new string[]
        {
            "Aku tenggelam di sungai tempat aku buang sampah.",
            "Ironis sekali, kan?",
            "Aku bukan suami dan ayah yang baik.",
            "Namun di akhir hidupku… aku hanya ingin dimaafkan."
        },

        new string[]
        {
            "Aku mati karena serangan jantung.",
            "Dalam perjalanan pulang setelah 18 jam bekerja.",
            "Aku mengejar uang… tapi kehilangan keluarga.",
            "Jika saja aku lebih menghargai waktu…"
        },

        new string[]
        {
            "Partner bisnisku membunuhku.",
            "Ia bilang aku tamak… tapi bukankah dia juga?",
            "Aku melakukan banyak kecurangan.",
            "Tapi aku juga membantu banyak orang berkembang."
        },

        new string[]
        {
            "Aku mati oleh penyakit yang tak bisa disembuhkan.",
            "Selama hidup, aku sering menyalahkan orang lain.",
            "Aku berkata kasar pada banyak orang tak bersalah.",
            "Aku menyesal… sangat menyesal."
        }
    };

    private List<int> remainingIndexes = new List<int>();

    public GameObject currentNPC;

    void Awake()
    {
        for (int i = 0; i < dialogSets.Count; i++)
            remainingIndexes.Add(i);
    }

    public GameObject SpawnNPC()
    {
        if (remainingIndexes.Count == 0)
        {
            Debug.Log("Tidak ada dialog tersisa.");
            return null;
        }

        int pick = remainingIndexes[Random.Range(0, remainingIndexes.Count)];
        remainingIndexes.Remove(pick);

        currentNPC = Instantiate(npcPrefab, pointLeft.parent);
        RectTransform rt = currentNPC.GetComponent<RectTransform>();
        rt.anchoredPosition = pointLeft.anchoredPosition;

        NPC_Controller ctrl = currentNPC.GetComponent<NPC_Controller>();
        ctrl.pointMiddle = pointMiddle;
        ctrl.pointRight = pointRight;

        DialogBubbleSpawner_Gameplay bubble = currentNPC.GetComponent<DialogBubbleSpawner_Gameplay>();
        bubble.dialogLines = dialogSets[pick];
        bubble.npc = ctrl;

        return currentNPC;
    }
}
