using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LevelFlow : GamemodeSubsystem
{
    [Header("Level Time")]
    [Tooltip("In seconds")]
    [SerializeField] private int levelDuration;

    private LevelEventSubsystem levelEventManager => TrainGameMode.GetLevelEventManager();
    private float currentLevelTime;
    private int currentLevelTimeRound => Mathf.RoundToInt(currentLevelTime);

    public override void OnUpdate()
    {
        currentLevelTime += Time.deltaTime;
    
        TrainGameMode.UpdateProgressBar(currentLevelTimeRound, levelDuration);

        levelEventManager.UpdateEventTimeline(currentLevelTimeRound);

        if (currentLevelTime >= levelDuration)
        {
            TrainGameMode.onWin();
        }
    }
    
    public int GetLevelDuration()
    {
        return levelDuration;
    }
}
