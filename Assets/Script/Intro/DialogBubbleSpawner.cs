using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public float fromLeftDuration = 1.2f;

    [Header("Typing Settings")]
    public float typingSpeed = 0.02f;

    [Header("Scene Transition")]
    public string nextSceneName = "MainMenu"; // 🔥 NAMA SCENE TUJUAN
    public float sceneDelay = 4f;             // 🔥 DELAY 4 DETIK

    private bool canStartDialog = false;
    private int index = -1;
    private bool isTyping = false;
    private bool dialogFinished = false;

    private GameObject currentBubble;
    private TMP_Text currentText;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (bossAnimator != null && !string.IsNullOrEmpty(fromLeftTrigger))
            bossAnimator.SetTrigger(fromLeftTrigger);

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
                FinishTypingImmediate();
            else
                ShowNextBubble();
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

        if (currentBubble != null)
            Destroy(currentBubble);

        if (bossAnimator != null && !string.IsNullOrEmpty(talkTrigger))
            bossAnimator.SetTrigger(talkTrigger);

        if (bubblePrefab != null && bubbleSpawnPoint != null)
        {
            currentBubble = Instantiate(
                bubblePrefab,
                bubbleSpawnPoint.position,
                Quaternion.identity,
                transform
            );
            currentText = currentBubble.GetComponentInChildren<TMP_Text>();
        }

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

        // 🔥 TRANSISI KE MAIN MENU
        StartCoroutine(GoToNextScene());
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(sceneDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
