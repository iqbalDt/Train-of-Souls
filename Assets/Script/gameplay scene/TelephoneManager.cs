using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TelephoneManager : MonoBehaviour
{
    public static TelephoneManager Instance;

    [Header("Telephone Subtitle UI")]
    public GameObject telephoneSubtitlePrefab;
    public RectTransform subtitleSpawnArea;

    [Header("Telephone Button")]
    public Button telephoneButton;

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.02f;

    private GameObject subtitleObj;
    private TMP_Text subtitleTMP;

    private bool isTyping = false;
    private bool hasUsedTelephoneForCurrentNPC = false;

    private string[] lines;
    private int lineIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    // RESET PER NPC — TELEPON HARUS NONAKTIF DI AWAL
    public void ResetTelephoneForNewNPC()
    {
        hasUsedTelephoneForCurrentNPC = false;
        ClearSubtitle();

        if (telephoneButton != null)
            telephoneButton.interactable = false;
    }

    public void CallRelative(GameObject npcObj)
    {
        if (hasUsedTelephoneForCurrentNPC)
            return;

        hasUsedTelephoneForCurrentNPC = true;

        var dialog = npcObj.GetComponent<DialogBubbleSpawner_Gameplay>();
        var topic  = dialog.GetActiveTopic();
        var state  = dialog.GetNPCState();

        string fullHint = GetHint(topic, state);

        lines = fullHint.Split('\n');
        lineIndex = 0;

        ShowSubtitle();

        telephoneButton.interactable = false;
    }

    string GetHint(DialogTopic topic, NPCState state)
    {
        switch (state)
        {
            case NPCState.Truth:  return topic.truthTelephoneHint;
            case NPCState.Lie:    return topic.lieTelephoneHint;
            default:              return topic.neutralTelephoneHint;
        }
    }

    void Update()
    {
        if (subtitleObj != null && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    void ShowSubtitle()
    {
        if (subtitleObj != null)
            Destroy(subtitleObj);

        subtitleObj = Instantiate(telephoneSubtitlePrefab, subtitleSpawnArea);
        subtitleTMP = subtitleObj.GetComponentInChildren<TMP_Text>();

        StopAllCoroutines();
        StartCoroutine(TypeLine(lines[lineIndex]));

        lineIndex++;
    }

    void ShowNextLine()
    {
        if (lineIndex >= lines.Length)
        {
            ClearSubtitle();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine(lines[lineIndex]));

        lineIndex++;
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        subtitleTMP.text = "";

        foreach (char c in text)
        {
            subtitleTMP.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    public void ClearSubtitle()
    {
        if (subtitleObj != null)
            Destroy(subtitleObj);

        subtitleObj = null;
    }
}
