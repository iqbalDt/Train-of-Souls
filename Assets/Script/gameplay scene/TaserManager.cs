using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TaserManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Taser UI")]
    public Image iconNormal;
    public Image iconActive;

    [Header("Lightning Window Effect")]
    public Image windowLightning;            // Image renderer di Canvas
    public Sprite[] lightningFrames;         // Kumpulan sprite animasi
    public float frameRate = 0.05f;          // Kecepatan animasi

    private Coroutine lightningRoutine;
    private bool isHolding = false;

    void Start()
    {
        // Pastikan lightning hidden di awal
        if (windowLightning != null)
        {
            Color c = windowLightning.color;
            c.a = 0f;
            windowLightning.color = c;
        }

        iconActive.gameObject.SetActive(false);
    }

    // =============================
    //  ON PRESS
    // =============================
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;

        iconActive.gameObject.SetActive(true);

        // Mulai animasi petir
        if (lightningRoutine != null)
            StopCoroutine(lightningRoutine);

        lightningRoutine = StartCoroutine(PlayLightningAnimation());
    }

    // =============================
    //  ON RELEASE
    // =============================
    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        iconActive.gameObject.SetActive(false);

        // Stop animasi
        if (lightningRoutine != null)
            StopCoroutine(lightningRoutine);

        StartCoroutine(FadeOutLightning());
    }

    // =============================
    //  SPRITE ANIMATION (Loop)
    // =============================
    IEnumerator PlayLightningAnimation()
    {
        int index = 0;

        // Buat lightning kelihatan
        Color c = windowLightning.color;
        c.a = 1f;
        windowLightning.color = c;

        while (isHolding)
        {
            // Ganti sprite frame
            windowLightning.sprite = lightningFrames[index];

            // Loop index
            index = (index + 1) % lightningFrames.Length;

            yield return new WaitForSeconds(frameRate);
        }
    }

    // =============================
    //  FADE OUT AFTER RELEASE
    // =============================
    IEnumerator FadeOutLightning()
    {
        Color c = windowLightning.color;

        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * 4f;
            windowLightning.color = c;
            yield return null;
        }
    }
}
