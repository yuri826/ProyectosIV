using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public static PlayerSystem instance;
    
    private PlayerMovement[] players = new PlayerMovement[4];

    private void Awake()
    {
        //playerMovement = GetComponent<PlayerMovement>();
        instance = this;
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

    public void ActivatePlayers()
    {
        foreach (PlayerMovement p in players)
        {
            if (p is not null) p.currentState = PlayerState.Move;
        }
    }

    public void DeactivatePlayers(PlayerState state)
    {
        foreach (PlayerMovement p in players)
        {
            if (p is not null) p.currentState = state;
        }
    }
}
