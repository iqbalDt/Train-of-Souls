using UnityEngine;

public class NPCVoiceProfile : MonoBehaviour
{
    [Header("Audio Source (optional)")]
    public AudioSource source;

    [Header("Main Dialog Voice by Emotion")]
    public AudioClip neutralVoice;
    public AudioClip happyVoice;
    public AudioClip sadVoice;
    public AudioClip madVoice;

    [Header("Reaction Mapping (defaults follow your rule)")]
    public NPCEmotion lieDetectorTruthEmotion = NPCEmotion.Happy; // Truth -> Happy
    public NPCEmotion lieDetectorLieEmotion = NPCEmotion.Mad;     // Lie -> Mad
    public NPCEmotion stunEmotion = NPCEmotion.Mad;               // Stun -> Mad

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    public bool stopPreviousBeforePlay = true;

    void Awake()
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D
        source.volume = volume;
    }

    public void PlayByEmotion(NPCEmotion emotion)
    {
        AudioClip clip = GetClipByEmotion(emotion);
        Play(clip);
    }

    public void PlayLieDetectorVoice(NPCState npcState)
    {
        // sesuai aturan kamu:
        // Truth -> Happy, Lie -> Mad, Neutral -> Neutral (default)
        NPCEmotion emotion = npcState switch
        {
            NPCState.Truth => lieDetectorTruthEmotion,
            NPCState.Lie => lieDetectorLieEmotion,
            _ => NPCEmotion.Neutral
        };

        PlayByEmotion(emotion);
    }

    public void PlayStunVoice()
    {
        PlayByEmotion(stunEmotion);
    }

    public void Stop()
    {
        if (source != null)
            source.Stop();
    }

    // =========================
    // INTERNAL
    // =========================

    AudioClip GetClipByEmotion(NPCEmotion emotion)
    {
        return emotion switch
        {
            NPCEmotion.Happy => happyVoice,
            NPCEmotion.Sad => sadVoice,
            NPCEmotion.Mad => madVoice,
            _ => neutralVoice
        };
    }

    void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;

        source.volume = volume;

        if (stopPreviousBeforePlay)
            source.Stop();

        source.PlayOneShot(clip);
    }
}
