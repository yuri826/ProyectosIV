using System.Collections.Generic;
using UnityEngine;

public class LevelEventManager : MonoBehaviour
{
    //DEPRECATED
//     [Header("Level Events")]
//     [SerializeField] private List<LevelEventData> levelEvents = new List<LevelEventData>();
//
//     [Header("External Systems")]
//     [SerializeField] private TrainSpawnDirector trainSpawnDirector;
//     [SerializeField] private SandstormSystem sandstormSystem;
//     [SerializeField] private CollapseSystem collapseSystem;
//
//     public void UpdateEventTimeline(int currentLevelTime)
//     {
//         for (int i = 0; i < levelEvents.Count; i++)
//         {
//             if (levelEvents[i].hasTriggered)
//             {
//                 continue;
//             }
//
//             if (currentLevelTime < levelEvents[i].triggerTime)
//             {
//                 continue;
//             }
//
//             TriggerEvent(levelEvents[i]);
//             levelEvents[i].hasTriggered = true;
//         }
//     }
//
//     private void TriggerEvent(LevelEventData eventData)
//     {
//         switch (eventData.eventType)
//         {
//             case LevelEventType.OutlawWave:
//                 TriggerOutlawWave(eventData);
//                 break;
//
//             case LevelEventType.Sandstorm:
//                 TriggerSandstorm(eventData);
//                 break;
//
//             case LevelEventType.Collapse:
//                 TriggerCollapse(eventData);
//                 break;
//         }
//     }
//
//     private void TriggerOutlawWave(LevelEventData eventData)
//     {
//         trainSpawnDirector.SpawnOutlawWave(eventData.outlawCount);
//     }
//
//     private void TriggerSandstorm(LevelEventData eventData)
//     {
//         sandstormSystem.StartSandstorm(eventData.duration);
//     }
//
//     private void TriggerCollapse(LevelEventData eventData)
//     {
//         collapseSystem.StartCollapse(eventData.duration, eventData.rockCount);
//     }
//
//     public List<LevelEventData> GetLevelEvents()
//     {
//         return levelEvents;
//     }
}
