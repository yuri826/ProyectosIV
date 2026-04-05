using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SandstormEvent",  menuName = "ScriptableObjects/LevelEvents/SandstormEvent")]
[Serializable]
public class SandstormEventData : LevelEventData
{
    private SandstormSystem sandstormSystem => SandstormSystem.Instance;
    
    public override void Execute()
    {
        base.Execute();
        
        sandstormSystem.StartSandstorm(duration);
    }
}
