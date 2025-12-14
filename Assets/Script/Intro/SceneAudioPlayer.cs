using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip audioClip;
    public bool playOnStart = true;
    public bool loop = true;
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D audio
    }

    void Start()
    {
        if (playOnStart && audioClip != null)
        {
            audioSource.Play();
        }
    }

    // OPTIONAL: kalau suatu saat mau stop manual
    public void StopAudio()
    {
        audioSource.Stop();
    }

    // OPTIONAL: play manual
    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
