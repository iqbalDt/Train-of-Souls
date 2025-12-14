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
    [Tooltip("Waktu diam setelah reaction selesai diketik")]
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
    // PUBLIC READ
    // =========================

    public bool IsDialogFinished() => dialogFinished;
    public DialogTopic GetActiveTopic() => activeTopic;
    public MoralValue GetCurrentMoralValue() => activeValue;
    public NPCState GetNPCState() => currentState;

    void Awake()
    {
        npcAnimator = GetComponent<NPC_AnimatorController>();
        voiceProfile = GetComponent<NPCVoiceProfile>(); // ✅ ambil dari NPC prefab yang sama
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

        // 🎭 set emosi di awal dialog
        if (npcAnimator != null)
            npcAnimator.SetEmotion(activeEmotion);

        // 🔊 MAIN DIALOG VOICE: berdasarkan emosi dialog (Happy/Sad/Mad/Neutral)
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

        bubbleObj = Instantiate(
            bubblePrefab,
            bubbleSpawnPoint.position,
            Quaternion.identity,
            transform
        );

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

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                bubbleTMP.rectTransform.parent as RectTransform
            );

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
    // REACTIONS (STUN & LIE)
    // =========================

    public void ShowStunReaction(string text)
    {
        // 🔊 STUN selalu pakai emosi marah (default Mad, bisa kamu ubah di Inspector)
        if (voiceProfile != null)
            voiceProfile.PlayStunVoice();

        StartCoroutine(ReactionRoutine(text));
    }

    public void ShowDetectorReaction(string text)
    {
        // 🔊 LieDetector: Truth -> Happy, Lie -> Mad (default), bisa kamu ubah mapping-nya di Inspector
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

        bubbleObj = Instantiate(
            bubblePrefab,
            bubbleSpawnPoint.position,
            Quaternion.identity,
            transform
        );

        bubbleTMP = bubbleObj.GetComponentInChildren<TMP_Text>();
        bubbleTMP.text = "";

        // NPC bicara
        if (npcAnimator != null)
            npcAnimator.SetSpeaking(true);

        // TYPEWRITER reaction (tidak bisa diskip)
        foreach (char c in text)
        {
            bubbleTMP.text += c;

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                bubbleTMP.rectTransform.parent as RectTransform
            );

            yield return new WaitForSeconds(typeSpeed);
        }

        if (npcAnimator != null)
            npcAnimator.SetSpeaking(false);

        yield return new WaitForSeconds(reactionHoldDuration);

        Destroy(bubbleObj);
        bubbleObj = null;

        allowTalking = prevAllowTalking;
    }
}
