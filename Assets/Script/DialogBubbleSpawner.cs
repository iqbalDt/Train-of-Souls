using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogBubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public Transform bubbleSpawnPoint;
    public string[] dialogLines;

    private int index = -1;
    private bool isTyping = false;

    private GameObject currentBubble;
    private TMP_Text currentText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            NextBubble();
        }
    }

    void NextBubble()
    {
        index++;
        if (index >= dialogLines.Length) return;

        if (currentBubble != null)
            Destroy(currentBubble);

        currentBubble = Instantiate(bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, transform);
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

            // 🔥 STEP PENTING: refresh di text, bukan di bubble
            LayoutRebuilder.ForceRebuildLayoutImmediate(currentText.rectTransform);

            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }
}
