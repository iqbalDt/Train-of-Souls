using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("Gameplay Tracking")]
    public int wrongChoices = 0;       // jumlah salah player
    public int totalNPC = 5;           // jumlah NPC yang harus dinilai
    private int judgedNPC = 0;         // NPC yang sudah dipilihkan surga/neraka

    [Header("Ending Tiers")]
    public EndingTier[] endingTiers;   // daftar ending berdasarkan skor

    [System.Serializable]
    public class EndingTier
    {
        public int minWrong;
        public int maxWrong;
        public string sceneName;       // nama scene ending untuk range ini
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Dipanggil setiap kali player membuat pilihan
    public void RegisterChoice(bool correct)
    {
        judgedNPC++;

        if (!correct)
        {
            wrongChoices++;
            Debug.Log("SALAH! Total salah sekarang: " + wrongChoices);
        }

        // Jika semua NPC sudah dinilai → tentukan ending
        if (judgedNPC >= totalNPC)
        {
            TriggerEnding();
        }
    }

    // Cek ending berdasarkan tiers
    void TriggerEnding()
    {
        foreach (var tier in endingTiers)
        {
            if (wrongChoices >= tier.minWrong &&
                wrongChoices <= tier.maxWrong)
            {
                Debug.Log("ENDING TRIGGERED → " + tier.sceneName);
                SceneManager.LoadScene(tier.sceneName);
                return;
            }
        }

        // fallback jika tidak ada tier yang cocok
        Debug.LogWarning("No ending tier matched! Add default tier.");
    }

    // Opsional reset jika ingin restart game
    public void ResetData()
    {
        wrongChoices = 0;
        judgedNPC = 0;
    }
}
