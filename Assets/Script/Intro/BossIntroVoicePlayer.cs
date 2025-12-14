using UnityEngine;

public class BossIntroVoicePlayer : MonoBehaviour
{
    [Header("References")]
    public DialogBubbleSpawner dialogSpawner;
    public Animator bossAnimator;

    [Header("Voice Settings")]
    public AudioClip voiceClip;
    public bool loopWhileTalking = true;
    public float volume = 1f;

    [Header("Animator Params")]
    public string talkTrigger = "Talk";
    public string stopTalkTrigger = "StopTalk";

    private AudioSource audioSource;
    private bool isTalking = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loopWhileTalking;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (dialogSpawner == null || bossAnimator == null)
            return;

        var stateInfo = bossAnimator.GetCurrentAnimatorStateInfo(0);

        // Kalau lagi anim Talk → play suara
        if (stateInfo.IsName(talkTrigger))
        {
            if (!isTalking)
            {
                PlayVoice();
                isTalking = true;
            }
        }
        else
        {
            if (isTalking)
            {
                StopVoice();
                isTalking = false;
            }
        }
    }

    void PlayVoice()
    {
        if (voiceClip == null) return;

        audioSource.clip = voiceClip;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void StopVoice()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
    public void AE_StartVoice()
    {
        PlayVoice();
    }

    public void AE_StopVoice()
    {
        StopVoice();
    }

}
