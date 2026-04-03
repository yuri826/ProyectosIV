using System;
using UnityEngine;

[Serializable]
public class PlayerSubsystem : GamemodeSubsystem
{
    [SerializeField] private PlayerMovement[] players = new PlayerMovement[4];

    public override void OnStart()
    {
        foreach (PlayerMovement p in players)
        {
            if (p is null) continue;
            
            activatePlayers += p.EnablePlayer;
            deactivatePlayers += p.DisablePlayer;
        }
    }

    public void SetState(PlayerState state, int playerN)
    {
        players[playerN].currentState = state;
    }

    public void ForcePick(PickableObj p, int playerN)
    {
        Debug.Log(playerN);
        players[playerN].ForcePick(p);
    }

    public void AddPlayer(PlayerMovement p, int playerN)
    {
        players[playerN] = p;
    }
    
    public PlayerMovement GetPlayer(int playerN)
    {
        if (playerN < 0 || playerN >= players.Length)
        {
            return null;
        }

        return players[playerN];
    }

    public delegate void ActivatePlayers();
    public ActivatePlayers activatePlayers;

    public void EndGameplay()
    {
        deactivatePlayers();
    }
    
    public delegate void DeactivatePlayers();
    public DeactivatePlayers deactivatePlayers;
}
