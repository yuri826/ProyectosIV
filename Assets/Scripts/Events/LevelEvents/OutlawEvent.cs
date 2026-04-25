using UnityEngine;

[CreateAssetMenu(fileName = "OutlawEvent", menuName = "Scriptable Objects/Level Events/OutlawEvent")]
public class OutlawEvent : LevelEvent
{
    [SerializeField] private int outlawCount;
    
    public override void ExecuteEvent()
    {
        TrainSpawnDirector.Instance.SpawnOutlawWave(outlawCount);
    }
}
