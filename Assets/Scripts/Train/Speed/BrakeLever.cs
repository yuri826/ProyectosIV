using UnityEngine;

public class BrakeLever : MonoBehaviour
{
    [Header("Brake")]
    [SerializeField] private float brakeDecayMultiplier = 3f;

    private PlayerMovement currentPlayer;
    private bool isHolded;

    public void OnHold(PlayerMovement player)
    {
        if (isHolded) return;

        currentPlayer = player;
        isHolded = true;

        TrainGameMode.instance.GetSpeedManager().SetBrakeMultiplier(brakeDecayMultiplier);
    }

    public void OnRelease(PlayerMovement player)
    {
        if (currentPlayer != player) return;

        ReleaseBrake();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isHolded) return;

        if (other.TryGetComponent(out PlayerMovement player) && player == currentPlayer)
        {
            ReleaseBrake();
            player.currentState = PlayerState.Move;
        }
    }

    private void ReleaseBrake()
    {
        isHolded = false;
        currentPlayer = null;

        TrainGameMode.instance.GetSpeedManager().ResetBrakeMultiplier();
    }
}
