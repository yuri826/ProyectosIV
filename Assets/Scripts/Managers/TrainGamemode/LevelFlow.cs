using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelFlow", menuName = "GameModeSubsystems/Train/LevelFlow")]
public class LevelFlow : GamemodeSubsystem
{
    [Header("Level Time")]
    [Tooltip("In seconds")]
    [SerializeField] private int levelDuration;
    private float currentLevelTime;
    private int currentLevelTimeRound => Mathf.RoundToInt(currentLevelTime);

    public override void OnUpdate()
    {
        currentLevelTime += Time.deltaTime;
        levelDuration = currentLevelTimeRound;

        if (currentLevelTime >= levelDuration)
        {
            TrainGameMode.Win();
        }
    }
}
