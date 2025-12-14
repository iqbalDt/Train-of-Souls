using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource loopSource;     // taser, ringing
    public AudioSource oneShotSource;  // klik, result, ticket

    [Header("FINAL VOLUME")]
    [Range(0f, 1f)] public float loopVolume = 0.7f;
    [Range(0f, 1f)] public float oneShotVolume = 0.7f;

    // =========================
    // TASER
    // =========================
    [Header("Taser SFX")]
    public AudioClip taserLoop;

    // =========================
    // TELEPHONE
    // =========================
    [Header("Telephone SFX")]
    public AudioClip phoneClick;
    public AudioClip phoneRingingLoop;
    public AudioClip phoneDisconnected;

    // =========================
    // LIE DETECTOR
    // =========================
    [Header("Lie Detector SFX")]
    public AudioClip lieDetectorClick;
    public AudioClip lieResult;
    public AudioClip truthResult;

    // =========================
    // HEAVEN / HELL
    // =========================
    [Header("Decision Button SFX")]
    public AudioClip heavenButton;
    public AudioClip hellButton;

    // =========================
    // TICKET
    // =========================
    [Header("Ticket SFX")]
    public AudioClip ticketPrint;
    public AudioClip ticketClick;
    public AudioClip ticketGiveToNPC;

    void Awake()
    {
        loopSource.volume = loopVolume;
        oneShotSource.volume = oneShotVolume;
    }

    // =========================
    // TASER
    // =========================
    public void StartTaser()
    {
        if (taserLoop == null) return;
        loopSource.clip = taserLoop;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopTaser()
    {
        if (loopSource.isPlaying)
            loopSource.Stop();
    }

    // =========================
    // TELEPHONE
    // =========================
    public void PlayPhoneClick()
    {
        PlayOneShot(phoneClick);
    }

    public void StartPhoneRinging()
    {
        if (phoneRingingLoop == null) return;
        loopSource.clip = phoneRingingLoop;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopPhoneRinging()
    {
        if (loopSource.isPlaying)
            loopSource.Stop();
    }

    public void PlayPhoneDisconnected()
    {
        PlayOneShot(phoneDisconnected);
    }

    // =========================
    // LIE DETECTOR
    // =========================
    public void PlayLieDetectorClick()
    {
        PlayOneShot(lieDetectorClick);
    }

    public void PlayLieResult()
    {
        PlayOneShot(lieResult);
    }

    public void PlayTruthResult()
    {
        PlayOneShot(truthResult);
    }

    // =========================
    // DECISION
    // =========================
    public void PlayHeaven()
    {
        PlayOneShot(heavenButton);
    }

    public void PlayHell()
    {
        PlayOneShot(hellButton);
    }

    // =========================
    // TICKET
    // =========================
    public void PlayTicketPrint()
    {
        PlayOneShot(ticketPrint);
    }

    public void PlayTicketClick()
    {
        PlayOneShot(ticketClick);
    }

    public void PlayTicketGiveToNPC()
    {
        PlayOneShot(ticketGiveToNPC);
    }

    // =========================
    // INTERNAL
    // =========================
    void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        oneShotSource.PlayOneShot(clip);
    }
}
