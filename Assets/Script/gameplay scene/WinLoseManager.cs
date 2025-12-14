using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("Ending Tiers")]
    public EndingTier[] endingTiers;

    [Header("Ending Delay")]
    public float endingDelay = 4f; // ✅ DELAY 4 DETIK

    // ================= INTERNAL =================
    private int wrongChoices = 0;
    private int judgedNPC = 0;
    private NPC_Spawner spawner;
    private bool endingTriggered = false;

    [System.Serializable]
    public class EndingTier
    {
        public int minWrong;
        public int maxWrong;
        public string sceneName;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        spawner = FindFirstObjectByType<NPC_Spawner>();
    }

    // ================= CALLED FROM GAMEFLOW =================
    public void RegisterChoice(bool correct)
    {
        judgedNPC++;

        if (!correct)
        {
            wrongChoices++;
            Debug.Log("❌ SALAH | Total Salah: " + wrongChoices);
        }
        else
        {
            Debug.Log("✅ BENAR");
        }

        // ✅ JIKA SEMUA NPC SUDAH DINILAI
        if (!endingTriggered && spawner != null && judgedNPC >= spawner.maxNPCPerDay)
        {
            endingTriggered = true;
            StartCoroutine(TriggerEndingAfterDelay());
        }
    }

    // ================= ENDING =================
    IEnumerator TriggerEndingAfterDelay()
    {
        Debug.Log("🎬 SEMUA NPC SELESAI — HITUNG ENDING");
        yield return new WaitForSeconds(endingDelay);

        EndingTier selectedTier = null;

        foreach (var tier in endingTiers)
        {
            if (wrongChoices >= tier.minWrong &&
                wrongChoices <= tier.maxWrong)
            {
                // pilih tier PALING SPESIFIK
                if (selectedTier == null || tier.minWrong > selectedTier.minWrong)
                    selectedTier = tier;
            }
        }

        if (selectedTier != null)
        {
            Debug.Log($"🏁 ENDING DIPILIH: {selectedTier.sceneName} | Salah: {wrongChoices}");
            SceneManager.LoadScene(selectedTier.sceneName);
        }
        else
        {
            Debug.LogError("❌ TIDAK ADA ENDING YANG COCOK — CEK ENDING TIERS!");
        }
    }

    // ================= OPTIONAL RESET =================
    public void ResetData()
    {
        wrongChoices = 0;
        judgedNPC = 0;
        endingTriggered = false;
    }
}
