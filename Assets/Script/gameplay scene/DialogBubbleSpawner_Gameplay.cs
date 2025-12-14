using UnityEngine;
using TMPro;
using System.Collections;

public class DialogBubbleSpawner_Gameplay : MonoBehaviour
{
    public enum MoralValue { Heaven, Hell, Neutral }

    [Header("List of Topics for this NPC")]
    public DialogTopic[] topics;
    [HideInInspector] public DialogTopic activeTopic;

    [Header("Bubble UI")]
    public GameObject bubblePrefab;
    public RectTransform bubbleSpawnPoint;

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.03f;
    public bool allowSkipTyping = true;

    private string[] activeTextLines;
    private MoralValue activeValue;
    private NPCEmotion activeEmotion;

    private bool allowTalking = false;
    private bool isTyping = false;
    private bool skipTyping = false;
    private int lineIndex = 0;

    private GameObject bubbleObj;
    private TMP_Text bubbleTMP;

    private NPCState currentState;
    private bool dialogFinished = false;

    private NPC_AnimatorController npcAnimator;

    // =========================

    public bool IsDialogFinished() => dialogFinished;
    public DialogTopic GetActiveTopic() => activeTopic;
    public MoralValue GetCurrentMoralValue() => activeValue;
    public NPCState GetNPCState() => currentState;

    void Awake()
    {
        npcAnimator = GetComponent<NPC_AnimatorController>();
    }

    public void AssignTopic(NPCState state)
    {
        currentState = state;
        dialogFinished = false;

        activeTopic = topics[Random.Range(0, topics.Length)];

        string rawText = "";

        switch (state)
        {
            case NPCState.Truth:
                rawText = activeTopic.truthText;
                activeValue = activeTopic.truthValue;
                activeEmotion = activeTopic.truthEmotion;
                break;

            case NPCState.Lie:
                rawText = activeTopic.lieText;
                activeValue = activeTopic.lieValue;
                activeEmotion = activeTopic.lieEmotion;
                break;

            default:
                rawText = activeTopic.neutralText;
                activeValue = activeTopic.neutralValue;
                activeEmotion = activeTopic.neutralEmotion;
                break;
        }

        activeTextLines = rawText.Split('\n');
        lineIndex = 0;
    }

    public void AllowTalking()
    {
        allowTalking = true;

        // 🎭 SET EMOTION SEKALI DI AWAL DIALOG
        if (npcAnimator != null)
            npcAnimator.SetEmotion(activeEmotion);

        ShowNextLine();
    }

    void Update()
    {
        if (!allowTalking) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping && allowSkipTyping)
                skipTyping = true;
            else if (!isTyping)
                ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (lineIndex >= activeTextLines.Length)
        {
            EndDialog();
            return;
        }

        if (bubbleObj != null)
            Destroy(bubbleObj);

        bubbleObj = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
        bubbleTMP = bubbleObj.GetComponentInChildren<TMP_Text>();

        StopAllCoroutines();
        StartCoroutine(TypeLine(activeTextLines[lineIndex]));

        lineIndex++;
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        skipTyping = false;

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(true);

        bubbleTMP.text = "";

        foreach (char c in text)
        {
            if (skipTyping) break;
            bubbleTMP.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        bubbleTMP.text = text;
        isTyping = false;

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(false);
    }

    void EndDialog()
    {
        allowTalking = false;
        dialogFinished = true;

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(false);

        if (bubbleObj != null)
            Destroy(bubbleObj);

        FindFirstObjectByType<GameFlowController>()?.OnDialogFinished();
    }

    // =========================
    // REACTIONS
    // =========================

    public void ShowStunReaction(string text, float duration = 1.2f)
    {
        StartCoroutine(ReactionRoutine(text, duration));
    }

    public void ShowDetectorReaction(string text, float duration = 1f)
    {
        StartCoroutine(ReactionRoutine(text, duration));
    }

    IEnumerator ReactionRoutine(string text, float duration)
    {
        bool prev = allowTalking;
        allowTalking = false;

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(true);

        GameObject obj = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
        TMP_Text tmp = obj.GetComponentInChildren<TMP_Text>();
        tmp.text = text;

        yield return new WaitForSeconds(duration);

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(false);

        Destroy(obj);
        allowTalking = prev;
    }
}
