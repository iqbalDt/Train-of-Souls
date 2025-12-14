using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TicketPrinter : MonoBehaviour
{
    [Header("Ticket References")]
    public RectTransform ticketHeaven;
    public RectTransform ticketHell;
    public RectTransform ticketSpawnPoint;
    public RectTransform ticketOutPoint; // posisi depan mesin
    public RectTransform npcTargetPoint; // arah terbang ke NPC

    [Header("Buttons")]
    public Button heavenButton;
    public Button hellButton;

    [Header("Animation")]
    public float printDuration = 0.4f;
    public float flyDuration = 0.5f;

    private RectTransform currentTicket;
    private GameFlowController gameFlow;

    void Awake()
    {
        gameFlow = FindObjectOfType<GameFlowController>();

        ticketHeaven.gameObject.SetActive(false);
        ticketHell.gameObject.SetActive(false);
    }

    // === DIPANGGIL DARI GameFlowController ===
    public void PrintHeavenTicket()
    {
        PrintTicket(ticketHeaven);
    }

    public void PrintHellTicket()
    {
        PrintTicket(ticketHell);
    }

    void PrintTicket(RectTransform ticket)
    {
        heavenButton.interactable = false;
        hellButton.interactable = false;

        currentTicket = ticket;
        ticket.gameObject.SetActive(true);
        ticket.position = ticketSpawnPoint.position;

        Button btn = ticket.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnTicketClicked);

        StartCoroutine(PrintAnimation());
    }

    IEnumerator PrintAnimation()
    {
        Vector3 start = ticketSpawnPoint.position;
        Vector3 end = ticketOutPoint.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / printDuration;
            currentTicket.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    void OnTicketClicked()
    {
        StartCoroutine(FlyToNPC());
    }

    IEnumerator FlyToNPC()
    {
        Vector3 start = currentTicket.position;
        Vector3 end = npcTargetPoint.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            currentTicket.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        currentTicket.gameObject.SetActive(false);
        currentTicket = null;

        gameFlow.OnTicketTaken();
    }

    // Dipanggil saat NPC baru masuk
    public void ResetPrinter()
    {
        heavenButton.interactable = true;
        hellButton.interactable = true;

        ticketHeaven.gameObject.SetActive(false);
        ticketHell.gameObject.SetActive(false);
    }
}
