using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TaserManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Taser UI")]
    public Image iconNormal;
    public Image iconActive;

    [Header("Lightning Effect")]
    public Image windowLightning;
    public Sprite[] lightningFrames;
    public float frameRate = 0.05f;

    [Header("Stun Settings")]
    public NPC_Spawner spawner;
    public int maxStunUses = 2;

    private int stunUses = 0;
    private bool isHolding = false;
    private Coroutine lightningRoutine;
    private GameObject stunnedNPC;

    void Start()
    {
        if (windowLightning != null)
        {
            Color c = windowLightning.color;
            c.a = 0f;
            windowLightning.color = c;
        }

        if (iconActive != null)
            iconActive.gameObject.SetActive(false);
    }

    // =========================
    // POINTER DOWN (START STUN)
    // =========================
    public void OnPointerDown(PointerEventData eventData)
    {
        if (stunUses >= maxStunUses) return;
        if (spawner == null || spawner.currentNPC == null) return;

        stunUses++;
        isHolding = true;

        if (iconActive != null)
            iconActive.gameObject.SetActive(true);

        if (lightningRoutine != null)
            StopCoroutine(lightningRoutine);

        lightningRoutine = StartCoroutine(PlayLightningAnimation());

        stunnedNPC = spawner.currentNPC;

        var anim = stunnedNPC.GetComponent<NPC_AnimatorController>();
        if (anim != null)
            anim.PlayStunEffect();

        if (stunUses >= maxStunUses && iconNormal != null)
        {
            Color c = iconNormal.color;
            c.a = 0.4f;
            iconNormal.color = c;
        }
    }

    // =========================
    // POINTER UP (END STUN)
    // =========================
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isHolding) return;

        isHolding = false;

        if (iconActive != null)
            iconActive.gameObject.SetActive(false);

        if (lightningRoutine != null)
            StopCoroutine(lightningRoutine);

        StartCoroutine(FadeOutLightning());

        if (stunnedNPC != null)
        {
            var anim = stunnedNPC.GetComponent<NPC_AnimatorController>();
            if (anim != null)
                anim.EndStun(); // 🔥 AUTO MAD

            var dialog = stunnedNPC.GetComponent<DialogBubbleSpawner_Gameplay>();
            if (dialog != null)
            {
                var topic = dialog.GetActiveTopic();
                if (topic != null && !string.IsNullOrEmpty(topic.stunReactionText))
                    dialog.ShowStunReaction(topic.stunReactionText);
            }

            stunnedNPC = null;
        }
    }

    // =========================
    // LIGHTNING FX
    // =========================
    IEnumerator PlayLightningAnimation()
    {
        if (windowLightning == null || lightningFrames == null || lightningFrames.Length == 0)
            yield break;

        int index = 0;
        Color c = windowLightning.color;
        c.a = 1f;
        windowLightning.color = c;

        while (isHolding)
        {
            windowLightning.sprite = lightningFrames[index];
            index = (index + 1) % lightningFrames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }

    IEnumerator FadeOutLightning()
    {
        if (windowLightning == null)
            yield break;

        Color c = windowLightning.color;
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * 4f;
            windowLightning.color = c;
            yield return null;
        }
    }
}
