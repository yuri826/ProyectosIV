using System;
using UnityEngine;

[Serializable]
public class LevelEventInfo
{
    [field: SerializeField] public int execTime { get; set; }
    [field: SerializeField] public LevelEvent levelEvent { get; set; }

    public bool HasBeenExecuted { get; private set; }

    public void Execute()
    {
        levelEvent.ExecuteEvent();
        HasBeenExecuted = true;
    }
}
