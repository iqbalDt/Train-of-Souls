using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogBubbleSpawner_Gameplay : MonoBehaviour
{
    [Header("Bubble Settings")]
    public GameObject bubblePrefab;
    public RectTransform bubbleSpawnPoint;
    public string[] dialogLines;

    [Header("NPC Reference")]
    public NPC_Controller npc;

    private int index = -1;
    private bool isTyping = false;
    private bool canTalk = false;

    private GameObject currentBubble;
    private TMP_Text currentText;

    void Update()
    {
        if (!canTalk) return;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isTyping)
        {
            NextBubble();
        }
    }

    public void AllowTalking()
    {
        canTalk = true;
        NextBubble();
    }

    public void NextBubble()
    {
        index++;

        if (index >= dialogLines.Length)
        {
            EndDialog();
            return;
        }

        if (currentBubble != null)
            Destroy(currentBubble);

        // spawn di canvas
        currentBubble = Instantiate(bubblePrefab, bubbleSpawnPoint.parent);

        // posisikan sesuai spawnpoint NPC
        currentBubble.GetComponent<RectTransform>().anchoredPosition =
            bubbleSpawnPoint.anchoredPosition;

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
            LayoutRebuilder.ForceRebuildLayoutImmediate(currentText.rectTransform);
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    void EndDialog()
    {
        if (currentBubble != null)
            Destroy(currentBubble);

        canTalk = false;

        // ❗ NPC tidak pergi otomatis lagi
        // Player harus memilih Surga atau Neraka
    }
}
