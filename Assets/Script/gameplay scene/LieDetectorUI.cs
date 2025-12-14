using UnityEngine;

public class LieDetectorUI : MonoBehaviour
{
    [Header("Animator")]
    public Animator anim;

    [Header("Audio")]
    public AudioSource audioSource;   // 1 AudioSource saja
    public AudioClip lieClip;         // suara saat LIE
    public AudioClip truthClip;       // suara saat TRUTH

    public void ShowLie()
    {
        // Animator
        anim.SetBool("Lie", true);
        anim.SetBool("Truth", false);
        anim.SetBool("Neutral", false);

        // Audio
        PlayLieSound();
    }

    public void ShowTruth()
    {
        // Animator
        anim.SetBool("Truth", true);
        anim.SetBool("Lie", false);
        anim.SetBool("Neutral", false);

        // Audio
        PlayTruthSound();
    }

    public void ShowNeutral()
    {
        // Animator
        anim.SetBool("Neutral", true);
        anim.SetBool("Lie", false);
        anim.SetBool("Truth", false);

        // ❌ tidak ada audio
    }

    // =========================
    // AUDIO HELPERS
    // =========================

    void PlayLieSound()
    {
        if (audioSource != null && lieClip != null)
            audioSource.PlayOneShot(lieClip);
    }

    void PlayTruthSound()
    {
        if (audioSource != null && truthClip != null)
            audioSource.PlayOneShot(truthClip);
    }
}
