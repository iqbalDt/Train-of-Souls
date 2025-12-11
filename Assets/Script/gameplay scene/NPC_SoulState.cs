using UnityEngine;

public class NPC_SoulState : MonoBehaviour
{
    [Header("NPC Behavior Flags")]
    public bool isLying;

    void Start()
    {
        // 50% peluang NPC berbohong
        isLying = Random.value < 0.5f;
    }
}
