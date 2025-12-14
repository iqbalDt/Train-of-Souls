using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

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

    [Header("Reaction Settings")]
    public float reactionHoldDuration = 1.8f;

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
    private NPCVoiceProfile voiceProfile;

    // =========================

    public bool IsDialogFinished() => dialogFinished;
    public DialogTopic GetActiveTopic() => activeTopic;
    public MoralValue GetCurrentMoralValue() => activeValue;
    public NPCState GetNPCState() => currentState;

    void Awake()
    {
        npcAnimator = GetComponent<NPC_AnimatorController>();
        voiceProfile = GetComponent<NPCVoiceProfile>();
    }

    // =========================
    // SETUP TOPIC
    // =========================

    public void AssignTopic(NPCState state)
    {
        currentState = state;
        dialogFinished = false;

        activeTopic = topics[Random.Range(0, topics.Length)];

        switch (state)
        {
            case NPCState.Truth:
                activeTextLines = activeTopic.truthText.Split('\n');
                activeValue = activeTopic.truthValue;
                activeEmotion = activeTopic.truthEmotion;
                break;

            case NPCState.Lie:
                activeTextLines = activeTopic.lieText.Split('\n');
                activeValue = activeTopic.lieValue;
                activeEmotion = activeTopic.lieEmotion;
                break;

            default:
                activeTextLines = activeTopic.neutralText.Split('\n');
                activeValue = activeTopic.neutralValue;
                activeEmotion = activeTopic.neutralEmotion;
                break;
        }

        lineIndex = 0;
    }

    // =========================
    // START DIALOG
    // =========================

    public void AllowTalking()
    {
        allowTalking = true;

        if (npcAnimator != null)
            npcAnimator.SetEmotion(activeEmotion);

        if (voiceProfile != null)
            voiceProfile.PlayByEmotion(activeEmotion);

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

    // =========================
    // MAIN DIALOG
    // =========================

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

        // 🔥 KUNCI: Speaking ON sepanjang typewriter
        if (npcAnimator != null)
            npcAnimator.SetSpeaking(true);

        bubbleTMP.text = "";

        foreach (char c in text)
        {
            if (skipTyping) break;

            bubbleTMP.text += c;

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                bubbleTMP.rectTransform.parent as RectTransform
            );

            yield return new WaitForSeconds(typeSpeed);
        }

        bubbleTMP.text = text;
        isTyping = false;

        // 🔥 BIARKAN TALK STATE HIDUP SEBENTAR
        yield return new WaitForSeconds(0.15f);

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
    // REACTIONS (STUN & LIE)
    // =========================

    public void ShowStunReaction(string text)
    {
        if (voiceProfile != null)
            voiceProfile.PlayStunVoice();

        StartCoroutine(ReactionRoutine(text));
    }

    public void ShowDetectorReaction(string text)
    {
        if (voiceProfile != null)
            voiceProfile.PlayLieDetectorVoice(currentState);

        StartCoroutine(ReactionRoutine(text));
    }

    IEnumerator ReactionRoutine(string text)
    {
        bool prevAllowTalking = allowTalking;
        allowTalking = false;

        if (bubbleObj != null)
            Destroy(bubbleObj);

        bubbleObj = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
        bubbleTMP = bubbleObj.GetComponentInChildren<TMP_Text>();
        bubbleTMP.text = "";

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(true);

        foreach (char c in text)
        {
            bubbleTMP.text += c;

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                bubbleTMP.rectTransform.parent as RectTransform
            );

            yield return new WaitForSeconds(typeSpeed);
        }

        // 🔥 TAHAN TALK SEDIKIT
        yield return new WaitForSeconds(0.15f);

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(false);

        yield return new WaitForSeconds(reactionHoldDuration);

        Destroy(bubbleObj);
        bubbleObj = null;

        allowTalking = prevAllowTalking;
    }
}
