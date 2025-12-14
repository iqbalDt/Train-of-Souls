using UnityEngine;
using System.Reflection;

public class BossVoicePlayer : MonoBehaviour
{
    [Header("Boss Voice Settings")]
    public AudioClip voiceClip;
    public bool loopWhileTalking = true;
    public float volume = 1f;

    [Header("Dialog Reference")]
    public BossEndingDialog dialog; // drag BossEndingDialog ke sini

    AudioSource audioSource;

    // reflection cache
    FieldInfo isTypingField;
    bool lastTypingState = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loopWhileTalking;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        if (dialog != null)
        {
            // ambil private field "isTyping"
            isTypingField = typeof(BossEndingDialog)
                .GetField("isTyping", BindingFlags.NonPublic | BindingFlags.Instance);

            if (isTypingField == null)
                Debug.LogError("[BossVoicePlayer] Field 'isTyping' not found!");
        }
        else
        {
            Debug.LogError("[BossVoicePlayer] BossEndingDialog reference is NULL");
        }
    }

    void Update()
    {
        if (dialog == null || isTypingField == null || voiceClip == null)
            return;

        bool isTypingNow = (bool)isTypingField.GetValue(dialog);

        // TRANSISI: false -> true (dialog mulai)
        if (isTypingNow && !lastTypingState)
        {
            PlayVoice();
        }

        // TRANSISI: true -> false (dialog selesai / diskip)
        if (!isTypingNow && lastTypingState)
        {
            StopVoice();
        }

        lastTypingState = isTypingNow;
    }

    void PlayVoice()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = voiceClip;
            audioSource.Play();
        }
    }

    void StopVoice()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
