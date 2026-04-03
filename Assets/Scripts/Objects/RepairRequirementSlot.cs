using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RepairRequirementSlot
{
    [Header("Requirement")]
    public ResourceType requiredResource;

    [Header("UI")]
    public Image slotImage;
    public Sprite emptySprite;
    public Sprite filledSprite;

    [HideInInspector] public bool isFilled = false;
}