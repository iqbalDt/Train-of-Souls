using UnityEngine;

public enum NPCEmotion
{
    Neutral = 0,
    Happy = 1,
    Sad = 2,
    Mad = 3
}

public class NPC_AnimatorController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    private NPCEmotion currentEmotion = NPCEmotion.Neutral;
    private bool isSpeaking;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        ApplyAnimator();
    }

    // =========================
    // PUBLIC API
    // =========================

    public void SetEmotion(NPCEmotion emotion)
    {
        // Mad adalah emosi terkunci
        if (currentEmotion == NPCEmotion.Mad && emotion != NPCEmotion.Mad)
            return;

        currentEmotion = emotion;
        ApplyAnimator();
    }

    public void SetSpeaking(bool value)
    {
        isSpeaking = value;
        ApplyAnimator();
    }

    public void ForceMadAndSpeak()
    {
        currentEmotion = NPCEmotion.Mad;
        isSpeaking = true;
        ApplyAnimator();
    }

    public void PlayStunEffect()
    {
        isSpeaking = false;
        ApplyAnimator();
    }

    public void EndStun()
    {
        currentEmotion = NPCEmotion.Mad;
        isSpeaking = false;
        ApplyAnimator();
    }

    // =========================
    // INTERNAL
    // =========================

    void ApplyAnimator()
    {
        if (animator == null) return;
        if (animator.runtimeAnimatorController == null) return;

        animator.SetInteger("Emotion", (int)currentEmotion);
        animator.SetBool("IsSpeaking", isSpeaking);
    }

    // =========================
    // RESET
    // =========================

    public void ResetAnimator()
    {
        currentEmotion = NPCEmotion.Neutral;
        isSpeaking = false;
        ApplyAnimator();
    }
}
