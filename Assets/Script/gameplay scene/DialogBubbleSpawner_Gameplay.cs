using UnityEngine;
using TMPro;
using System.Collections;

public class DialogBubbleSpawner_Gameplay : MonoBehaviour
{
    [System.Serializable]
    public struct DialogEntry
    {
        [TextArea(3,10)]
        public string text;

        public MoralValue moralValue;
    }

    public enum MoralValue
    {
        Heaven,
        Hell,
        Neutral
    }

    [Header("Bubble Prefab")]
    public GameObject bubblePrefab;
    public RectTransform bubbleSpawnPoint;

    [Header("Dialog Sets")]
    public DialogEntry[] truthDialogs;
    public DialogEntry[] lieDialogs;
    public DialogEntry[] neutralDialogs;

    private DialogEntry[] chosenDialogs;
    private int index = -1;

    private bool allowTalking = false;
    private bool isTyping = false;

    private GameObject currentBubble;
    private TMP_Text currentText;

    private MoralValue currentDialogValue;

    public void SetupDialog(NPCState state)
    {
        switch (state)
        {
            case NPCState.Truth:
                chosenDialogs = truthDialogs;
                break;

            case NPCState.Lie:
                chosenDialogs = lieDialogs;
                break;

            case NPCState.Neutral:
                chosenDialogs = neutralDialogs;
                break;
        }

        // fallback jika kosong
        if (chosenDialogs == null || chosenDialogs.Length == 0)
        {
            chosenDialogs = new DialogEntry[]
            {
                new DialogEntry { text = "...", moralValue = MoralValue.Neutral }
            };
        }
    }

    public MoralValue GetCurrentMoralValue()
    {
        return currentDialogValue;
    }

    public void AllowTalking()
    {
        index = -1;
        allowTalking = true;
        NextBubble();
    }

    void Update()
    {
        if (!allowTalking) return;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isTyping)
            NextBubble();
    }

    public void NextBubble()
    {
        index++;

        if (index >= chosenDialogs.Length)
        {
            EndDialog();
            return;
        }

        if (currentBubble != null)
            Destroy(currentBubble);

        currentBubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
        currentText = currentBubble.GetComponentInChildren<TMP_Text>();

        currentDialogValue = chosenDialogs[index].moralValue;

        StopAllCoroutines();
        StartCoroutine(TypeText(chosenDialogs[index].text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentText.text = "";

        foreach (char c in text)
        {
            currentText.text += c;
            yield return new WaitForSeconds(0.015f);
        }

        isTyping = false;
    }

    void EndDialog()
    {
        if (currentBubble != null)
            Destroy(currentBubble);

        allowTalking = false;

        FindFirstObjectByType<GameFlowController>().OnDialogFinished();
    }
}
