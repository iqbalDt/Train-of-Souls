using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    [Header("Movement Points")]
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    [Header("NPC State")]
    public NPCState npcState;
    public float speed = 300f;

    // ===== INTERNAL =====
    private RectTransform rt;
    private bool reachedMiddle = false;
    private bool exiting = false;

    private DialogBubbleSpawner_Gameplay dialog;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        dialog = GetComponent<DialogBubbleSpawner_Gameplay>();
    }

    void Update()
    {
        // =========================
        // MASUK KE TENGAH
        // =========================
        if (!reachedMiddle)
        {
            Move(pointMiddle.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointMiddle.anchoredPosition) < 2f)
            {
                reachedMiddle = true;

                // 🔥 PENTING:
                // 1️⃣ Notify GameFlow DULU (reset state, UI, dll)
                GameFlowController.Instance?.OnNPCReachedMiddle(this);

                // 2️⃣ BARU mulai dialog (ini yang FIX animasi talking)
                dialog.AllowTalking();
            }
        }
        // =========================
        // KELUAR KE KANAN
        // =========================
        else if (exiting)
        {
            Move(pointRight.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointRight.anchoredPosition) < 2f)
            {
                Destroy(gameObject);
            }
        }
    }

    // =========================
    // MOVE HELPER
    // =========================
    void Move(Vector2 target)
    {
        rt.anchoredPosition = Vector2.MoveTowards(
            rt.anchoredPosition,
            target,
            speed * Time.deltaTime
        );
    }

    // =========================
    // EXIT API
    // =========================
    public void StartExitMovement()
    {
        exiting = true;
    }
}
