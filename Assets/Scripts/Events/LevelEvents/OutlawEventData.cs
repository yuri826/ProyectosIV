using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OutlawEvent", menuName = "ScriptableObjects/LevelEvents/OutlawEvent")]
[Serializable]
public class OutlawEventData : LevelEventData
{
    [SerializeField] private int outlawCount;
    private TrainSpawnDirector trainSpawnDirector => TrainSpawnDirector.Instance;
    
    public override void Execute()
    {
        base.Execute();
        
        trainSpawnDirector.SpawnOutlawWave(outlawCount);
    }
}
