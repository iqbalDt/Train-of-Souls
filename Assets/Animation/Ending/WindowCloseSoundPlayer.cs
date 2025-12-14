using UnityEngine;

public class WindowCloseSoundPlayer : MonoBehaviour
{
    [Header("References")]
    public Animator rollerAnimator;

    [Header("Animation Settings")]
    public string windowCloseStateName = "WindowClose";

    [Header("Sound Settings")]
    public AudioClip windowCloseSound;
    public float volume = 1f;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D
    }

    void Update()
    {
        if (rollerAnimator == null || hasPlayed) return;

        AnimatorStateInfo state = rollerAnimator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName(windowCloseStateName) || state.IsName("WindowClose"))
        {
            PlaySound();
            hasPlayed = true;
        }
    }

    void PlaySound()
    {
        if (windowCloseSound == null) return;
        audioSource.PlayOneShot(windowCloseSound);
    }
}
