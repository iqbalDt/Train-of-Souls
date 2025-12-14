using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WindowSoundPlayer : MonoBehaviour
{
    [Header("Window Sound")]
    public AudioClip windowCloseSound;
    public float volume = 1f;
    public bool playOnce = true;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    // === DIPANGGIL DARI ANIMATION EVENT ===
    public void AE_PlayWindowCloseSound()
    {
        if (windowCloseSound == null) return;

        if (playOnce && hasPlayed) return;

        audioSource.clip = windowCloseSound;
        audioSource.Play();

        hasPlayed = true;
    }
}
