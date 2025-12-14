using UnityEngine;
using TMPro;
using System.Collections;

public class TelephoneManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject telephoneUI;
    public TMP_Text telephoneText;

    [Header("Style")]
    public int callerFontSize = 28;
    public int dialogFontSize = 24;
    public Color textColor = Color.white;

    [Header("Timing")]
    public float callingDuration = 2f;
    public float typeSpeed = 0.02f;

    [Header("Audio")]
    public AudioSource ringingSource;   // LOOP
    public AudioSource oneShotSource;   // ONESHOT
    public AudioClip disconnectClip;

    // ===== INTERNAL =====
    private string[] lines;
    private int lineIndex;
    private bool isActive;
    private bool isTyping;
    private string callerHeader;
    private Coroutine currentRoutine;

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

        string rawText = dialog.GetNPCState() switch
        {
            NPCState.Truth => topic.truthTelephoneHint,
            NPCState.Lie => topic.lieTelephoneHint,
            _ => topic.neutralTelephoneHint
        };

        if (string.IsNullOrEmpty(rawText)) return;

        lines = rawText.Split('\n');
        lineIndex = 0;

        string callerName = string.IsNullOrEmpty(topic.callerName)
            ? "UNKNOWN"
            : topic.callerName.ToUpper();

        callerHeader = $"<size={callerFontSize}><b>[{callerName}]</b></size>";

        telephoneText.color = textColor;
        telephoneText.fontSize = dialogFontSize;
        telephoneText.richText = true;

        telephoneUI.SetActive(true);
        isActive = true;

        // 🔊 START RINGING
        if (ringingSource != null && !ringingSource.isPlaying)
            ringingSource.Play();

        currentRoutine = StartCoroutine(CallingRoutine());
    }

    IEnumerator CallingRoutine()
    {
        telephoneText.text = "Calling...";
        yield return new WaitForSeconds(callingDuration);

        StopRinging();
        ShowNextLine();
    }

    void ShowNextLine()
    {
        // 🔇 PASTIKAN RINGING MATI JIKA PLAYER SKIP
        StopRinging();

        if (lines == null || lineIndex >= lines.Length)
        {
            EndTelephone();
            return;
        }

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(TypeLine(lines[lineIndex]));
        lineIndex++;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        telephoneText.text = callerHeader + "\n";

        foreach (char c in line)
        {
            telephoneText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    void EndTelephone()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        isActive = false;
        isTyping = false;

        StopRinging();

        // 📵 DISCONNECT
        if (oneShotSource != null && disconnectClip != null)
            oneShotSource.PlayOneShot(disconnectClip);

        if (telephoneUI != null)
            telephoneUI.SetActive(false);

        if (telephoneText != null)
            telephoneText.text = "";

        lines = null;
        lineIndex = 0;
    }

    public void ForceClose()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        StopRinging();

        isActive = false;
        isTyping = false;

        if (telephoneUI != null)
            telephoneUI.SetActive(false);

        if (telephoneText != null)
            telephoneText.text = "";

        lines = null;
        lineIndex = 0;
    }

    void StopRinging()
    {
        if (ringingSource != null && ringingSource.isPlaying)
            ringingSource.Stop();
    }
}
