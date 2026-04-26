using System;
using UnityEngine;

[Serializable]
public class LevelEventInfo
{
    [field: SerializeField] public int execTime { get; private set; }
    [field: SerializeField] public LevelEvent levelEvent { get; private set; }

    public bool HasBeenExecuted { get; private set; }

    public void Execute()
    {
        levelEvent.ExecuteEvent();
        HasBeenExecuted = true;
    }
}
