using UnityEngine;
using TMPro;
using System.Collections;

public class TelephoneManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject telephoneUI;      // TelephoneManager/TelephoneUI
    public TMP_Text telephoneText;      // TelephoneManager/TelephoneUI/Text (TMP)

    [Header("Style")]
    public int callerFontSize = 28;
    public int dialogFontSize = 24;
    public Color textColor = Color.white;

    [Header("Timing")]
    public float callingDuration = 2f;
    public float typeSpeed = 0.02f;

    // ===== INTERNAL =====
    private string[] lines;
    private int lineIndex;
    private bool isActive;
    private bool isTyping;
    private string callerHeader;

    void Awake()
    {
        if (telephoneUI != null)
            telephoneUI.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;
        if (isTyping) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    // =========================
    // PUBLIC API
    // =========================

    public void StartTelephone(GameObject npc)
    {
        if (isActive) return;
        if (npc == null) return;

        var dialog = npc.GetComponent<DialogBubbleSpawner_Gameplay>();
        if (dialog == null) return;

        var topic = dialog.GetActiveTopic();
        if (topic == null) return;

        string callerName = string.IsNullOrEmpty(topic.callerName)
            ? "UNKNOWN"
            : topic.callerName.ToUpper();

        string rawText = dialog.GetNPCState() switch
        {
            NPCState.Truth => topic.truthTelephoneHint,
            NPCState.Lie => topic.lieTelephoneHint,
            _ => topic.neutralTelephoneHint
        };

        if (string.IsNullOrEmpty(rawText)) return;

        lines = rawText.Split('\n');
        lineIndex = 0;

        // ✅ TMP-CORRECT TAG (INI FIX UTAMA)
        callerHeader = $"<size={callerFontSize}><b>[{callerName}]</b></size>";

        StopAllCoroutines();
        StartCoroutine(CallSequence());
    }

    // =========================
    // CORE FLOW
    // =========================

    IEnumerator CallSequence()
    {
        isActive = true;
        telephoneUI.SetActive(true);

        telephoneText.color = textColor;
        telephoneText.fontSize = dialogFontSize;
        telephoneText.textWrappingMode = TextWrappingModes.Normal;
        telephoneText.richText = true;

        telephoneText.text = "Calling...";
        yield return new WaitForSeconds(callingDuration);

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (lines == null || lineIndex >= lines.Length)
        {
            EndTelephone();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine(lines[lineIndex]));
        lineIndex++;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        string current = "";

        foreach (char c in line)
        {
            current += c;
            telephoneText.text = callerHeader + "\n" + current;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    void EndTelephone()
    {
        StopAllCoroutines();

        isActive = false;
        isTyping = false;

        telephoneUI.SetActive(false);
        telephoneText.text = "";

        lines = null;
        lineIndex = 0;
    }

    // Dipanggil saat NPC ganti (safety)
    public void ForceClose()
    {
        StopAllCoroutines();

        isActive = false;
        isTyping = false;

        if (telephoneUI != null)
            telephoneUI.SetActive(false);

        if (telephoneText != null)
            telephoneText.text = "";

        lines = null;
        lineIndex = 0;
    }

    public bool IsTelephoneActive()
    {
        return isActive;
    }
}
