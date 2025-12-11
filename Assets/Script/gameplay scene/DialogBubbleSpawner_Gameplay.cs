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

    private string[] activeTextLines;
    private MoralValue activeValue;

    private bool allowTalking = false;
    private bool isTyping = false;
    private int lineIndex = 0;

    private GameObject bubbleObj;
    private TMP_Text bubbleTMP;

    private NPCState currentState;

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.02f;

    // ============================
    //      TOPIC ASSIGNMENT
    // ============================
    public void AssignTopic(NPCState state)
    {
        currentState = state;

        if (topics == null || topics.Length == 0)
        {
            Debug.LogWarning("NPC tidak punya topic!");
            return;
        }

        activeTopic = topics[Random.Range(0, topics.Length)];
        string rawText = "";

        switch (state)
        {
            case NPCState.Truth:
                rawText = activeTopic.truthText;
                activeValue = activeTopic.truthValue;
                break;

            case NPCState.Lie:
                rawText = activeTopic.lieText;
                activeValue = activeTopic.lieValue;
                break;

            case NPCState.Neutral:
                rawText = activeTopic.neutralText;
                activeValue = activeTopic.neutralValue;
                break;
        }

        // multi-line support
        activeTextLines = rawText.Split('\n');
        lineIndex = 0;
    }

    public MoralValue GetCurrentMoralValue() => activeValue;
    public DialogTopic GetActiveTopic() => activeTopic;
    public NPCState GetNPCState() => currentState;

    // ============================
    //      TALK FLOW
    // ============================
    public void AllowTalking()
    {
        allowTalking = true;
        ShowNextLine();
    }

    void Update()
    {
        if (!allowTalking) return;

        // SPACE untuk lanjut dialog NPC
        if (!isTyping && Input.GetKeyDown(KeyCode.Space))
        {
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
        bubbleTMP.text = "";

        foreach (char c in text)
        {
            bubbleTMP.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        // NPC MENUNGGU SPACE, jadi tidak ada auto-next
    }

    void EndDialog()
    {
        allowTalking = false;

        if (bubbleObj != null)
            Destroy(bubbleObj);

        FindFirstObjectByType<GameFlowController>().OnDialogFinished();
    }
}
