using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

//Maneja el transcurso del tiempo en el nivel
public class LevelFlow : GamemodeSubsystem
{
    [Header("Level Time")]
    [Tooltip("In seconds")]
    [field: SerializeField] public int levelDuration { get; private set; }

    private float currentLevelTime;
    private int currentLevelTimeRound => Mathf.RoundToInt(currentLevelTime);

    public override void OnUpdate()
    {
        //Suma tiempo al nivel
        currentLevelTime += Time.deltaTime;
    
        //Updatea la timeline y el progreso en el gamemanager
        TrainGameMode.UpdateProgressBar(currentLevelTimeRound, levelDuration);
        TrainGameMode.GetLevelEventSubsystem().UpdateEventTimeline(currentLevelTimeRound);
    
        //Termina el nivel si el tiempo es mayor a tal
        if (currentLevelTime >= levelDuration) TrainGameMode.onWin();
    }
}
