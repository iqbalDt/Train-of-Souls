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

    [Header("Intro Animation")]
    public string fromLeftTrigger = "FromLeft";
    public float fromLeftDuration = 1.2f; // durasi animasi dari kiri

    [Header("Typing Settings")]
    public float typingSpeed = 0.02f;

    private bool canStartDialog = false;
    private int index = -1;
    private bool isTyping = false;
    private bool dialogFinished = false;

    private GameObject currentBubble;
    private TMP_Text currentText;
    private Coroutine typingCoroutine;

    void Start()
    {
        // MAININ ANIM MUNCUL DARI KIRI
        if (bossAnimator != null && !string.IsNullOrEmpty(fromLeftTrigger))
            bossAnimator.SetTrigger(fromLeftTrigger);

        // Aktifkan dialog setelah anim dari kiri selesai
        StartCoroutine(EnableDialogAfterDelay(fromLeftDuration));
    }

    IEnumerator EnableDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canStartDialog = true;
    }

    void Update()
    {
        if (!canStartDialog || dialogFinished)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                FinishTypingImmediate();
            }
            else
            {
                ShowNextBubble();
            }
        }
    }

    void ShowNextBubble()
    {
        index++;

        if (index >= dialogLines.Length)
        {
            EndDialog();
            return;
        }

        // Hapus bubble sebelumnya
        if (currentBubble != null)
            Destroy(currentBubble);

        // Trigger anim Boss Talk
        if (bossAnimator != null && !string.IsNullOrEmpty(talkTrigger))
            bossAnimator.SetTrigger(talkTrigger);

        // Spawn bubble baru
        if (bubblePrefab != null && bubbleSpawnPoint != null)
        {
            currentBubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
            currentText = currentBubble.GetComponentInChildren<TMP_Text>();
        }

        // Start typing
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogLines[index]));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        if (currentText == null)
        {
            yield return new WaitForSeconds(typingSpeed * fullText.Length);
            isTyping = false;
            yield break;
        }

        currentText.text = "";

        foreach (char c in fullText)
        {
            currentText.text += c;
            LayoutRebuilder.ForceRebuildLayoutImmediate(currentText.rectTransform);
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // Trigger anim Boss StopTalk setelah selesai mengetik
        if (bossAnimator != null && !string.IsNullOrEmpty(stopTalkTrigger))
            bossAnimator.SetTrigger(stopTalkTrigger);

        typingCoroutine = null;
    }

    void FinishTypingImmediate()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (currentText != null && index < dialogLines.Length)
            currentText.text = dialogLines[index];

        isTyping = false;

        if (bossAnimator != null && !string.IsNullOrEmpty(stopTalkTrigger))
            bossAnimator.SetTrigger(stopTalkTrigger);

        typingCoroutine = null;
    }

    void EndDialog()
    {
        dialogFinished = true;

        if (currentBubble != null)
            Destroy(currentBubble);

        if (bossAnimator != null)
        {
            if (!string.IsNullOrEmpty(stopTalkTrigger))
                bossAnimator.SetTrigger(stopTalkTrigger);

            if (!string.IsNullOrEmpty(moveTrigger))
                bossAnimator.SetTrigger(moveTrigger);
        }
    }
}
