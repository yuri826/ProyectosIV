using System;
using UnityEngine;

[Serializable]

//Maneja información sobre los jugadores en escena
public class PlayerSubsystem : GamemodeSubsystem
{
    [SerializeField] private PlayerMovement[] players = new PlayerMovement[4];

    public override void OnStart()
    {
        //Suscribe a los players a los eventos de activar y desactivar jugadores
        foreach (PlayerMovement p in players)
        {
            if (p is null) continue;
            
            activatePlayers += p.EnablePlayer;
            deactivatePlayers += p.DisablePlayer;
        }
    }

    //Manipula los estados del jugador
    public void SetState(PlayerState state, int playerN)
    {
        players[playerN].currentState = state;
    }

    public void ForcePick(PickableObj p, int playerN)
    {
        players[playerN].ForcePick(p);
    }
    
    //Para recoger un jugador con el index del array
    public PlayerMovement GetPlayer(int playerN)
    {
        if (playerN < 0 || playerN >= players.Length)
        {
            return null;
        }

        return players[playerN];
    }

    //Delegados de activar y desactivar players
    public delegate void ActivatePlayers();
    public ActivatePlayers activatePlayers;

    public void EndGameplay()
    {
        deactivatePlayers();
    }
    
    public delegate void DeactivatePlayers();
    public DeactivatePlayers deactivatePlayers;
}
