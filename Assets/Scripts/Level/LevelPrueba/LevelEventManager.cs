using System.Collections.Generic;
using UnityEngine;

public class LevelEventManager : MonoBehaviour
{
    [Header("Level Events")]
    [SerializeField] private List<LevelEventData> levelEvents = new List<LevelEventData>();

    [Header("External Systems")]
    [SerializeField] private TrainSpawnDirector trainSpawnDirector;

    public void UpdateEventTimeline(int currentLevelTime)
    {
        for (int i = 0; i < levelEvents.Count; i++)
        {
            if (levelEvents[i].hasTriggered)
            {
                continue;
            }

            if (currentLevelTime < levelEvents[i].triggerTime)
            {
                continue;
            }

            TriggerEvent(levelEvents[i]);
            levelEvents[i].hasTriggered = true;
        }
    }

    private void TriggerEvent(LevelEventData eventData)
    {
        switch (eventData.eventType)
        {
            case LevelEventType.OutlawWave:
                TriggerOutlawWave(eventData);
                break;

            case LevelEventType.Sandstorm:
                TriggerSandstorm(eventData);
                break;
        }
    }

    private void TriggerOutlawWave(LevelEventData eventData)
    {
        trainSpawnDirector.SpawnOutlawWave(eventData.outlawCount);
    }

    private void TriggerSandstorm(LevelEventData eventData)
    {
        Debug.Log("Sandstorm triggered. Duration: " + eventData.duration);
    }
}
