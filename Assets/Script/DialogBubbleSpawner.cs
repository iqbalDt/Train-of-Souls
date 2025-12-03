using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogBubbleSpawner : MonoBehaviour
{
    [Header("Bubble Settings")]
    public GameObject bubblePrefab;
    public Transform bubbleSpawnPoint;
    public string[] dialogLines;

    [Header("Boss Settings")]
    public Animator bossAnimator;   
    public string talkTrigger = "Talk";
    public string stopTalkTrigger = "StopTalk";
    public string moveTrigger = "StartMove";

    private int index = -1;
    private bool isTyping = false;
    private bool dialogFinished = false;

    private GameObject currentBubble;
    private TMP_Text currentText;

    void Update()
    {
        if (dialogFinished)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            NextBubble();
        }
    }

    // 👉 DIBUAT PUBLIC AGAR BISA DIPANGGIL DARI SCRIPT LAIN
    public void NextBubble()
    {
        index++;

        // kalau dialog sudah selesai
        if (index >= dialogLines.Length)
        {
            EndDialog();
            return;
        }

        // hapus bubble lama
        if (currentBubble != null)
            Destroy(currentBubble);

        // 🔊 mulai animasi BOS NGOMONG
        if (bossAnimator != null)
            bossAnimator.SetTrigger(talkTrigger);

        // spawn bubble
        currentBubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
        currentText = currentBubble.GetComponentInChildren<TMP_Text>();

        StopAllCoroutines();
        StartCoroutine(TypeText(dialogLines[index]));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        currentText.text = "";

        foreach (char c in fullText)
        {
            currentText.text += c;

            LayoutRebuilder.ForceRebuildLayoutImmediate(currentText.rectTransform);
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;

        // 🔇 Setelah teks selesai, BOS BERHENTI NGOMONG
        if (bossAnimator != null)
            bossAnimator.SetTrigger(stopTalkTrigger);
    }

    // ----------------------------------------
    // 👉 FUNGSI SAAT DIALOG SELESAI
    // ----------------------------------------
    void EndDialog()
    {
        dialogFinished = true;

        if (currentBubble != null)
            Destroy(currentBubble);

        // 🔇 stop talk (jaga2 kalau belum sempat stop)
        if (bossAnimator != null)
            bossAnimator.SetTrigger(stopTalkTrigger);

        // Tambah logic lain di sini jika perlu
        // 🔥 BOS JALAN KE KANAN
        if (bossAnimator != null)
            bossAnimator.SetTrigger(moveTrigger);
    }
}
