using System.Collections.Generic;
using UnityEngine;

public class BrakeLever : MonoBehaviour
{
    [Header("Brake")]
    [SerializeField] private float brakeDecayMultiplier = 3f;

    private readonly List<PlayerMovement> playersInRange = new();

    private void Update()
    {
        if (!IsAnyPlayerHoldingInteract())
        {
            return;
        }

        TrainGameMode.instance.GetSpeedManager().ApplyBrakeMultiplier(brakeDecayMultiplier);
    }

    private bool IsAnyPlayerHoldingInteract()
    {
        for (int i = 0; i < playersInRange.Count; i++)
        {
            if (playersInRange[i].IsHoldingInteract)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerMovement player))
        {
            return;
        }

        if (playersInRange.Contains(player))
        {
            return;
        }

        playersInRange.Add(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerMovement player))
        {
            return;
        }

        playersInRange.Remove(player);
    }
}
