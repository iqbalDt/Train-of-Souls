using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    public NPCState npcState;
    public float speed = 300f;

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
        if (!reachedMiddle)
        {
            Move(pointMiddle.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointMiddle.anchoredPosition) < 2f)
            {
                reachedMiddle = true;

                dialog.AllowTalking();
                FindFirstObjectByType<GameFlowController>()?.OnNPCReachedMiddle();
            }
        }
        else if (exiting)
        {
            Move(pointRight.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointRight.anchoredPosition) < 2f)
                Destroy(gameObject);
        }
    }

    void Move(Vector2 target)
    {
        rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, target, speed * Time.deltaTime);
    }

    public void StartExitMovement() => exiting = true;
}
