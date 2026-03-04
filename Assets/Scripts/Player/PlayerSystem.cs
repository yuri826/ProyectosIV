using System;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public static PlayerSystem instance;
    
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        instance = this;
    }

    public void SetState(string state)
    {
        playerMovement.state = state;
    }

    public void ForcePick(PickableObj p)
    {
        playerMovement.ForcePick(p);
    }
}
