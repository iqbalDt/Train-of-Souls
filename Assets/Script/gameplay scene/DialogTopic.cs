using UnityEngine;

[System.Serializable]
public class DialogTopic
{
    public string topicName;

    [Header("=== TRUTH VERSION ===")]
    [TextArea(3,10)] public string truthText;
    public DialogBubbleSpawner_Gameplay.MoralValue truthValue;
    [TextArea(3,10)] public string truthTelephoneHint;

    [Header("=== LIE VERSION ===")]
    [TextArea(3,10)] public string lieText;
    public DialogBubbleSpawner_Gameplay.MoralValue lieValue;
    [TextArea(3,10)] public string lieTelephoneHint;

    [Header("=== NEUTRAL VERSION ===")]
    [TextArea(3,10)] public string neutralText;
    public DialogBubbleSpawner_Gameplay.MoralValue neutralValue;
    [TextArea(3,10)] public string neutralTelephoneHint;
}
