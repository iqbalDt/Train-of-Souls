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
    public Animator bossAnimator;   // 👉 drag animator BOS ke sini
    public string bossMoveTrigger = "StartMove";  // 👉 nama trigger di Animator

    private int index = -1;
    private bool isTyping = false;

    private GameObject currentBubble;
    private TMP_Text currentText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            NextBubble();
        }
    }

    void NextBubble()
    {
        index++;

        // 👉 Kalau dialog sudah selesai
        if (index >= dialogLines.Length)
        {
            EndDialog();
            return;
        }

        // Hapus bubble sebelumnya
        if (currentBubble != null)
            Destroy(currentBubble);

        // Spawn bubble baru
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

            // Refresh layout text
            LayoutRebuilder.ForceRebuildLayoutImmediate(currentText.rectTransform);

            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    // ----------------------------------------
    // 👉 BAGIAN TERPENTING: FUNGSI DIALOG SELESAI
    // ----------------------------------------
    void EndDialog()
    {
        // Hapus bubble terakhir kalau ada
        if (currentBubble != null)
            Destroy(currentBubble);

        // 🔥 Trigger animasi bos jalan
        if (bossAnimator != null)
            bossAnimator.SetTrigger(bossMoveTrigger);

        // Bisa ditambah logic lain:
        // - ganti scene
        // - munculin UI baru
        // - play sound
    }
}
