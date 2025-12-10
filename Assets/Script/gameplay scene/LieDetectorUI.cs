using UnityEngine;

public class LieDetectorUI : MonoBehaviour
{
    public Animator anim;

    public void ShowLie()
    {
        anim.SetBool("Lie", true);
        anim.SetBool("Truth", false);
        anim.SetBool("Neutral", false);
    }

    public void ShowTruth()
    {
        anim.SetBool("Truth", true);
        anim.SetBool("Lie", false);
        anim.SetBool("Neutral", false);
    }

    public void ShowNeutral()
    {
        anim.SetBool("Neutral", true);
        anim.SetBool("Lie", false);
        anim.SetBool("Truth", false);
    }
}
