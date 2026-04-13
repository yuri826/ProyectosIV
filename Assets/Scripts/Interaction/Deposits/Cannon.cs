using UnityEngine;

public class Cannon : DepositObj
{
    //On tool -> playerState = cannon
    //On tool -> playerState = move

    private CannonState state = CannonState.PlayerOff;
    private int currentPlayer = -4;
    
    public void OnTool(int playerN)
    {
        if (state == CannonState.PlayerOff)
        {
            currentPlayer = playerN;
            TrainGameMode.instance.SetPlayerState(PlayerState.Cannon, playerN);
        }
        else
        {
            TrainGameMode.instance.SetPlayerState(PlayerState.Move, currentPlayer);
            currentPlayer = -4;
        }
    }
}
