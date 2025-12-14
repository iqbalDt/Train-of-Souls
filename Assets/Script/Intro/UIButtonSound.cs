using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        // Buat AudioSource otomatis
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound untuk UI
        audioSource.volume = volume;

        // Pasang listener ke Button
        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
    }
}
