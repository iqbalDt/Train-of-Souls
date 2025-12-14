using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossEndingDialog : MonoBehaviour
{
    [System.Serializable]
    public class EndingDialogGroup
    {
        public string endingSceneName;
        public string[] dialogLines;
        public bool isHappyMood = true;
    }

    [Header("Bubble Settings")]
    public GameObject bubblePrefab;
    public Transform bubbleSpawnPoint;

    [Header("Ending Dialog Groups")]
    public EndingDialogGroup[] allEndings;
    private string[] activeDialog;
    private bool activeIsHappy = true;

    [Header("Animator")]
    public Animator bossAnimator;
    public string appearTrigger = "AppearFromLeft";
    public string happyTrigger = "Happy";
    public string angryTrigger = "Angry";
    public string moveRightTrigger = "MoveRight";

    [Header("Animator State Names")]
    public string happyStateName = "Boss_Happy";
    public string angryStateName = "Boss_Angry";

    [Header("Timing")]
    public float appearDuration = 1.0f;
    public float fallbackMoodDuration = 0.9f;

    [Header("Roller Animator (Window Close)")]
    public Animator rollerAnimator;
    public string windowCloseTrigger = "WindowClose";

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; 
    public float sceneChangeDelay = 3f;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup; // CanvasGroup parent UI main menu
    public float fadeDuration = 1.5f; // lama fade out

    // private states
    GameObject bubble;
    TMP_Text bubbleText;

    int index = -1;
    bool isTyping = false;
    bool animPlaying = false;
    bool dialogFinished = false;
    bool canStart = false;

    Coroutine typeRoutine;
    Coroutine animWaitRoutine;

    void Start()
    {
        if (bubbleSpawnPoint == null)
            bubbleSpawnPoint = this.transform;

        PrintAvailableEndings();
        ChooseEndingDialog();

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f; // awal transparan

        if (bossAnimator != null && AnimatorHasParameter(bossAnimator, appearTrigger))
            bossAnimator.SetTrigger(appearTrigger);

        StartCoroutine(EnableStartAfterAppear());
    }

    void PrintAvailableEndings()
    {
        if (allEndings == null || allEndings.Length == 0) return;
        string list = "[BossEndingDialog] Available endings:";
        foreach (var e in allEndings)
        {
            string name = string.IsNullOrEmpty(e.endingSceneName) ? "<empty>" : e.endingSceneName;
            int lines = (e.dialogLines == null) ? 0 : e.dialogLines.Length;
            list += $"\n - {name} (lines: {lines}, happy:{e.isHappyMood})";
        }
        Debug.Log(list);
    }

    void ChooseEndingDialog()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (var e in allEndings)
        {
            if (!string.IsNullOrEmpty(e.endingSceneName) && e.endingSceneName == currentScene && e.dialogLines.Length > 0)
            {
                activeDialog = e.dialogLines;
                activeIsHappy = e.isHappyMood;
                return;
            }
        }
        activeDialog = new string[] { "No dialog configured for this ending." };
        activeIsHappy = true;
    }

    IEnumerator EnableStartAfterAppear()
    {
        yield return new WaitForSeconds(Mathf.Max(0.01f, appearDuration));
        canStart = true;
    }

    void Update()
    {
        if (!canStart || dialogFinished) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) { FinishTypingImmediate(); return; }
            if (animPlaying) return;
            StartCoroutine(HandleLine());
        }
    }

    IEnumerator HandleLine()
    {
        index++;
        if (index >= activeDialog.Length) { yield return StartCoroutine(EndDialog()); yield break; }

        if (bubble != null) Destroy(bubble);

        if (bubblePrefab != null)
        {
            bubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
            bubbleText = bubble.GetComponentInChildren<TMP_Text>();
        }
        else bubbleText = null;

        string trigger = activeIsHappy ? happyTrigger : angryTrigger;
        string state = activeIsHappy ? happyStateName : angryStateName;

        float animDuration = fallbackMoodDuration;
        if (bossAnimator != null && AnimatorHasParameter(bossAnimator, trigger))
        {
            bossAnimator.SetTrigger(trigger);
            animDuration = TryGetClipLengthForState(state, fallbackMoodDuration);
            if (animWaitRoutine != null) StopCoroutine(animWaitRoutine);
            animWaitRoutine = StartCoroutine(WaitForAnimFinish(animDuration));
        }

        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = StartCoroutine(TypeText(activeDialog[index]));

        while (isTyping || animPlaying) yield return null;
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        if (bubbleText == null) { yield return new WaitForSeconds(0.02f * text.Length); isTyping = false; yield break; }
        bubbleText.text = "";
        foreach (char c in text)
        {
            bubbleText.text += c;
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleText.rectTransform);
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
        typeRoutine = null;
    }

    void FinishTypingImmediate()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        if (bubbleText != null && index < activeDialog.Length)
            bubbleText.text = activeDialog[index];
        isTyping = false;
        typeRoutine = null;
    }

    IEnumerator WaitForAnimFinish(float duration)
    {
        animPlaying = true;
        float t = 0f;
        while (t < duration) { t += Time.deltaTime; yield return null; }
        animPlaying = false;
        animWaitRoutine = null;
    }

    IEnumerator EndDialog()
    {
        dialogFinished = true;
        if (bubble != null) Destroy(bubble);

        // Boss move right
        if (bossAnimator != null && AnimatorHasParameter(bossAnimator, moveRightTrigger))
        {
            bossAnimator.SetTrigger(moveRightTrigger);
            float moveDuration = TryGetClipLengthForState("Boss_MoveRight", 0.7f); 
            yield return new WaitForSeconds(moveDuration);
        }

        // Roller Window Close
        float rollerDuration = 0.7f;
        if (rollerAnimator != null && AnimatorHasParameter(rollerAnimator, windowCloseTrigger))
        {
            rollerAnimator.SetTrigger(windowCloseTrigger);
            if (rollerAnimator.runtimeAnimatorController != null)
            {
                foreach (var clip in rollerAnimator.runtimeAnimatorController.animationClips)
                {
                    if (clip.name.Contains("WindowClose"))
                    {
                        rollerDuration = clip.length;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(rollerDuration);
        }

        // Fade out CanvasGroup
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(FadeOutCanvasGroup(fadeCanvasGroup, fadeDuration));

        // Delay tambahan
        yield return new WaitForSeconds(sceneChangeDelay);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    IEnumerator FadeOutCanvasGroup(CanvasGroup cg, float duration)
    {
        if (cg == null) yield break;

        float t = 0f;
        float startAlpha = cg.alpha;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 1f, t / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    bool AnimatorHasParameter(Animator anim, string paramName)
    {
        if (anim == null || string.IsNullOrEmpty(paramName)) return false;
        foreach (var p in anim.parameters)
            if (p.name == paramName) return true;
        return false;
    }

    float TryGetClipLengthForState(string stateName, float fallback)
    {
        if (bossAnimator == null || string.IsNullOrEmpty(stateName)) return fallback;
        var controller = bossAnimator.runtimeAnimatorController;
        if (controller == null) return fallback;
        foreach (var clip in controller.animationClips)
            if (clip.name == stateName || clip.name.Contains(stateName))
                return clip.length;
        return fallback;
    }
}
