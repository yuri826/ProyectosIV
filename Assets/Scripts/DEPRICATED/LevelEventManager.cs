//
//DEPRECATED
//
// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// [Serializable]
// public class LevelEventManager : GamemodeSubsystem
// {
//     [Header("Level Events")]
//     [SerializeField] private List<LevelEventInstance> levelEvents = new List<LevelEventInstance>();
//
//     
//     [SerializeField] private SandstormSystem sandstormSystem;
//
//     public void UpdateEventTimeline(int currentLevelTime)
//     {
//         foreach (var myEvent in levelEvents)
//         {
//             if ((myEvent.hasTriggered) || (currentLevelTime < myEvent.timeToSpawn))
//             {
//                 continue;
//             }
//             
//             //Es mejor juntar los ifs para control de flujo si no tienes nada que se ejecute entre ambos
//             // if (currentLevelTime < levelEvents[i].timeToSpawn)
//             // {
//             //     continue;
//             // }
//
//             //Ahora manejado desde el evento
//             // TriggerEvent(levelEvents[i].eventData);
//             // levelEvents[i].eventData.hasTriggered = true;
//             
//             myEvent.ExecuteEvent();
//         }
//     }
//     //
//     // private void TriggerEvent(LevelEventData eventData)
//     // {
//     //     switch (eventData.eventType)
//     //     {
//     //         case LevelEventType.OutlawWave:
//     //             TriggerOutlawWave(eventData);
//     //             break;
//     //
//     //         case LevelEventType.Sandstorm:
//     //             TriggerSandstorm(eventData);
//     //             break;
//     //     }
//     // }
//
//     // private void TriggerOutlawWave(LevelEventData eventData)
//     // {
//     //     trainSpawnDirector.SpawnOutlawWave(eventData.outlawCount);
//     // }
//     //
//     // private void TriggerSandstorm(LevelEventData eventData)
//     // {
//     //     sandstormSystem.StartSandstorm(eventData.duration);
//     // }
//     
//     public List<LevelEventData> GetLevelEvents()
//     {
//         List<LevelEventData> events = new List<LevelEventData>();
//
//         foreach (var myEvent in levelEvents)
//         {
//             events.Add(myEvent.eventData);
//         }
//         return events;
//     }
// }
