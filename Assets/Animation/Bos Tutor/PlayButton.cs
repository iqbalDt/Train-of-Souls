using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButton : MonoBehaviour
{
    public AudioClip clickSound;
    public string sceneToLoad = "Intro";

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
    }

    public void OnPlayClicked()
    {
        Time.timeScale = 1f; // 🔥 WAJIB
        StartCoroutine(PlayAndLoad());
    }

    IEnumerator PlayAndLoad()
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSecondsRealtime(clickSound.length);
        SceneManager.LoadScene(sceneToLoad);
    }
}
