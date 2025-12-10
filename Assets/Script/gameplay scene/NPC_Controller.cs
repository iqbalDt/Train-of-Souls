using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    [Header("Movement Points")]
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    [Header("NPC State")]
    public NPCState npcState;

    [Header("Settings")]
    public float speed = 300f;

    private RectTransform rt;
    private bool reachedMiddle = false;
    private bool exiting = false;

    private DialogBubbleSpawner_Gameplay bubble;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        bubble = GetComponent<DialogBubbleSpawner_Gameplay>();
    }

    void Update()
    {
        if (!reachedMiddle)
        {
            MoveTo(pointMiddle.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointMiddle.anchoredPosition) < 2f)
            {
                reachedMiddle = true;

                // Mulai dialog
                bubble.AllowTalking();

                // beri tahu gameflow
                var flow = FindFirstObjectByType<GameFlowController>();
                if (flow != null)
                    flow.OnNPCReachedMiddle();

                return;
            }
        }
        else if (exiting)
        {
            MoveTo(pointRight.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointRight.anchoredPosition) < 2f)
            {
                Destroy(gameObject);
            }
        }
    }

    void MoveTo(Vector2 target)
    {
        rt.anchoredPosition =
            Vector2.MoveTowards(rt.anchoredPosition, target, speed * Time.deltaTime);
    }

    public void StartExitMovement()
    {
        exiting = true;
    }
}
