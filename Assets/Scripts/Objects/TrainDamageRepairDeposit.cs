using UnityEngine;
using UnityEngine.UI;

public class TrainDamageRepairDeposit : DepositObj
{
    [Header("Target Damage")]
    [SerializeField] private SabotagePoint targetSabotagePoint;

    [Header("Repair Settings")]
    [SerializeField] private float repairedLifeAmount = 10f;

    [Header("Requirements")]
    [SerializeField] private RepairRequirementSlot[] requirementSlots;

    protected override void Start()
    {
        base.Start();
        ResetRepairDeposit();
    }

    public override void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;

        if (currentState != DepositState.Objects)
        {
            return;
        }

        for (int i = 0; i < requirementSlots.Length; i++)
        {
            if (requirementSlots[i].isFilled)
            {
                continue;
            }

            if (pickableObj.type != requirementSlots[i].requiredResource)
            {
                continue;
            }

            requirementSlots[i].isFilled = true;
            UpdateRequirementSlotVisual(i);

            isCorrectObject = true;
            break;
        }

        if (AreAllRequirementsFilled())
        {
            currentState = DepositState.Tool;
        }
    }

    protected override void Completed()
    {
        base.Completed();

        if (targetSabotagePoint != null)
        {
            targetSabotagePoint.RepairPoint();
        }

        if (TrainGameMode.instance != null)
        {
            float repairAmount = repairedLifeAmount;

            if (targetSabotagePoint != null)
            {
                repairAmount = targetSabotagePoint.GetDamageValue();
            }

            TrainGameMode.instance.RepairTrain(repairAmount);
        }

        ResetRepairDeposit();
    }

    public void ResetRepairDeposit()
    {
        for (int i = 0; i < requirementSlots.Length; i++)
        {
            requirementSlots[i].isFilled = false;
            ResetRequirementSlotVisual(i);
        }

        objectIndex = 0;
        currentState = DepositState.Objects;
        repairTimer = 0f;
        repairing = false;
        currentPlayer = 0;

        if (repairBar != null)
        {
            repairBar.SetActive(false);
        }

        if (repairBarImage != null)
        {
            repairBarImage.fillAmount = 0f;
        }
    }

    private bool AreAllRequirementsFilled()
    {
        if (requirementSlots == null || requirementSlots.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < requirementSlots.Length; i++)
        {
            if (!requirementSlots[i].isFilled)
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateRequirementSlotVisual(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= requirementSlots.Length)
        {
            return;
        }

        if (requirementSlots[slotIndex].slotImage == null)
        {
            return;
        }

        if (requirementSlots[slotIndex].filledSprite == null)
        {
            return;
        }

        requirementSlots[slotIndex].slotImage.sprite = requirementSlots[slotIndex].filledSprite;
    }

    private void ResetRequirementSlotVisual(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= requirementSlots.Length)
        {
            return;
        }

        if (requirementSlots[slotIndex].slotImage == null)
        {
            return;
        }

        if (requirementSlots[slotIndex].emptySprite == null)
        {
            return;
        }

        requirementSlots[slotIndex].slotImage.sprite = requirementSlots[slotIndex].emptySprite;
    }
}
