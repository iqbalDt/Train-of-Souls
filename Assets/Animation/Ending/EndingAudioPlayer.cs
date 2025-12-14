using UnityEngine;

public class EndingAudioPlayer : MonoBehaviour
{
    [Header("Ending Audio Settings")]
    public AudioClip endingAudio;

    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
    }

    void Start()
    {
        PlayEndingAudio();
    }

    public void PlayEndingAudio()
    {
        if (endingAudio == null)
        {
            Debug.LogWarning("[EndingAudioPlayer] No audio clip assigned.");
            return;
        }

        audioSource.PlayOneShot(endingAudio);
    }
}
