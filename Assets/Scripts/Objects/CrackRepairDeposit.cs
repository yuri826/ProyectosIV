using UnityEngine;
using UnityEngine.UI;

public class CrackRepairDeposit : DepositObj
{
    [Header("Crack Repair")]
    [SerializeField] private SabotagePoint sabotagePoint;
    [SerializeField] private float repairedLifeAmount;

    [Header("Required Visuals")]
    [SerializeField] private Image plankSlotImage;
    [SerializeField] private Image nailSlotImage;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private Sprite plankFilledSprite;
    [SerializeField] private Sprite nailFilledSprite;

    private bool hasPlank;
    private bool hasNail;

    protected override void Start()
    {
        base.Start();
        ResetDeposit();
    }

    public override void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;

        if (currentState != DepositState.Objects)
        {
            return;
        }

        if (pickableObj.type == ResourceType.Planks && !hasPlank)
        {
            hasPlank = true;
            isCorrectObject = true;
            UpdateSlotVisuals();
        }
        else if (pickableObj.type == ResourceType.Nail && !hasNail)
        {
            hasNail = true;
            isCorrectObject = true;
            UpdateSlotVisuals();
        }

        if (hasPlank && hasNail)
        {
            currentState = DepositState.Tool;
        }
    }

    protected override void Completed()
    {
        base.Completed();

        if (sabotagePoint != null)
        {
            sabotagePoint.RepairPoint();
        }

        TrainGameMode.instance.RepairTrain(repairedLifeAmount);

        ResetDeposit();
    }

    private void ResetDeposit()
    {
        hasPlank = false;
        hasNail = false;
        objectIndex = 0;
        currentState = DepositState.Objects;
        repairTimer = 0f;
        repairing = false;

        if (repairBar != null)
        {
            repairBar.SetActive(false);
        }

        if (repairBarImage != null)
        {
            repairBarImage.fillAmount = 0f;
        }

        UpdateSlotVisuals();
    }

    private void UpdateSlotVisuals()
    {
        if (plankSlotImage != null)
        {
            plankSlotImage.sprite = hasPlank ? plankFilledSprite : emptySlotSprite;
        }

        if (nailSlotImage != null)
        {
            nailSlotImage.sprite = hasNail ? nailFilledSprite : emptySlotSprite;
        }
    }
}
