using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Button Sound Settings")]
    public AudioClip buttonClickSound;
    public float delayBeforeLoad = 0.2f; // delay minimal supaya suara terdengar

    public void LoadScene(string sceneName)
    {
        // pastikan AudioManager ada
        if (AudioManager.instance != null && buttonClickSound != null)
        {
            AudioManager.instance.PlaySound(buttonClickSound);
        }

        // delay sebelum load scene supaya suara terdengar
        StartCoroutine(LoadAfterDelay(sceneName));
    }

    private IEnumerator LoadAfterDelay(string sceneName)
    {
        float waitTime = buttonClickSound != null ? buttonClickSound.length : delayBeforeLoad;
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }
}
