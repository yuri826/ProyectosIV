using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BrakeLever : MonoBehaviour
{
    [Header("Brake")]
    [SerializeField] private float brakeDecayMultiplier = 3f;
    
    private bool isHolded = false;

    private int currentPlayer;

    public void OnHold()
    {
        if (isHolded) return;
        isHolded = true;
    }

    public void OnRelease()
    {
        isHolded = false;
        TrainGameMode.instance.GetPlayer(currentPlayer).currentState = PlayerState.Move;
    }

    private void Update()
    {
        if (isHolded) TrainGameMode.instance.GetSpeedManager().ApplyBrakeMultiplier(brakeDecayMultiplier);
    }
}
