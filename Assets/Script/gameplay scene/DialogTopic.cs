using UnityEngine;

[System.Serializable]
public class DialogTopic
{
    public string topicName;

    [Header("=== MAIN DIALOG TEXT ===")]
    [TextArea] public string truthText;
    [TextArea] public string lieText;
    [TextArea] public string neutralText;

    [Header("=== MORAL VALUE ===")]
    public DialogBubbleSpawner_Gameplay.MoralValue truthValue;
    public DialogBubbleSpawner_Gameplay.MoralValue lieValue;
    public DialogBubbleSpawner_Gameplay.MoralValue neutralValue;

    [Header("=== NPC EMOTION WHILE SPEAKING ===")]
    public NPCEmotion truthEmotion = NPCEmotion.Neutral;
    public NPCEmotion lieEmotion = NPCEmotion.Mad;
    public NPCEmotion neutralEmotion = NPCEmotion.Neutral;

    [Header("=== NPC VOICE PROFILE ===")]
    public NPCVoiceProfile voiceProfile;

    [Header("=== TELEPHONE CALLER ===")]
    public string callerName = "Unknown";

    [Header("=== TELEPHONE HINTS ===")]
    [TextArea] public string truthTelephoneHint;
    [TextArea] public string lieTelephoneHint;
    [TextArea] public string neutralTelephoneHint;

    [Header("=== REACTION TO LIE DETECTOR ===")]
    [TextArea] public string reactionTruth = "Tuh kan! Aku jujur!";
    [TextArea] public string reactionLie = "H-Hah?! Aku ga bohong!";
    [TextArea] public string reactionNeutral = "Hmm...";

    [Header("=== REACTION TO TASER ===")]
    [TextArea] public string stunReactionText = "#$%@!&^#";
}
