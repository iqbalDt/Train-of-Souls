using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public RectTransform pointMiddle;
    public RectTransform pointRight;

    public float speed = 300f;

    private RectTransform rt;
    private bool reachedMiddle = false;
    private bool exiting = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!reachedMiddle)
        {
            MoveTo(pointMiddle.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointMiddle.anchoredPosition) < 5f)
            {
                reachedMiddle = true;

                // Mulai dialog
                GetComponent<DialogBubbleSpawner_Gameplay>().AllowTalking();
            }
        }
        else if (exiting)
        {
            MoveTo(pointRight.anchoredPosition);

            if (Vector2.Distance(rt.anchoredPosition, pointRight.anchoredPosition) < 5f)
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

    // Dipanggil saat player memilih Surga/Neraka
    public void OnDialogFinished()
    {
        exiting = true;
    }
}
