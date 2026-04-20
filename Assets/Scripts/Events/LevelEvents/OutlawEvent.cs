using UnityEngine;

public class OutlawEvent : LevelEvent
{
    [SerializeField] private int outlawCount;
    
    public override void ExecuteEvent()
    {
        TrainSpawnDirector.Instance.SpawnOutlawWave(outlawCount);
    }
}
