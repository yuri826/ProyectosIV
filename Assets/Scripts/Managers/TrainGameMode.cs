using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class TrainGameMode : MonoBehaviour
{
    public static TrainGameMode instance;
    
    [Header("Components")]
    [SerializeField] private float introTime;
    [SerializeField] private UIUpdater uiUpdater;
    [SerializeField] private LevelFlow levelFlow;
    [SerializeField] private TrainLife trainLife;
    [FormerlySerializedAs("speedManager")] [SerializeField] private SpeedManagerSubsystem speedManager;
    [SerializeField] private PlayerSubsystem playerSystem;
    
    [Header("Gameplay")]
    private LevelFlowState currentState = LevelFlowState.Intro;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        uiUpdater.TrainGameMode = this;
        levelFlow.TrainGameMode = this;
        playerSystem.TrainGameMode = this;
        trainLife.TrainGameMode = this;
    
        uiUpdater.OnStart();
        levelFlow.OnStart();
        playerSystem.OnStart();
        speedManager.OnStart();
        trainLife.OnStart();

        onWin += Win;
        onWin += uiUpdater.OnWin;
        onWin += levelFlow.OnWin;
        onWin += playerSystem.EndGameplay;
    
        onGameOver += GameOver;
        onGameOver += uiUpdater.OnGameOver;
        onGameOver += levelFlow.OnGameOver;
        onGameOver += playerSystem.EndGameplay;
        
        StartCoroutine(uiUpdater.IntroRoutine(introTime)); //Bindear corrutina?
    }

    public void Update()
    {
        switch (currentState)
        {
            case LevelFlowState.Gameplay:
                
                uiUpdater.OnUpdate();
                levelFlow.OnUpdate();
                
                break;
        }
    }

    //GameState
    public void StartGameplay()
    {
        currentState = LevelFlowState.Gameplay;
        playerSystem.activatePlayers();
        speedManager.StartStartup();
    }
    
    private void Win()
    {
        currentState = LevelFlowState.Win;
        Time.timeScale = 0;
    }
    
    private void GameOver()
    {
        currentState = LevelFlowState.GameOver;
        Time.timeScale = 0;
    }

    //UI
    public void UpdateProgressBar(int progress, int maxProgress)
    {
        uiUpdater.UpdateProgressBar(progress, maxProgress);
    }
    
    public void UpdateLifeBar(float currentLife, int maxLife)
    {
        uiUpdater.UpdateLifeBar(currentLife, maxLife);
    }

    //PlayerSystem
    public void SetPlayerState(PlayerState state, int currentPlayer)
    {
        playerSystem.SetState(state, currentPlayer);
    }
    
    public void ForcePick(PickableObj p, int playerN)
    {
        playerSystem.ForcePick(p, playerN);
    }
    
    public PlayerMovement GetPlayer(int playerN)
    {
        return playerSystem.GetPlayer(playerN);
    }
    
    //Health
    public void TakeDamage(float damage)
    {
        trainLife.TakeDamage(damage);
    }
    
    public void RepairTrain(float amount)
    {
        trainLife.RepairTrain(amount);
    }
    
    public float GetCurrentTrainLife()
    {
        return trainLife.GetCurrentTrainLife();
    }

    public int GetMaxTrainLife()
    {
        return trainLife.GetMaxTrainLife();
    }

    public float GetCurrentSpeed()
    {
        return speedManager.GetCurrentSpeed();
    }
    
    //Speed
    
    public void AddSpeed(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        if (speedManager.CoalBoostRoutine != null)
        {
            StopCoroutine(speedManager.CoalBoostRoutine);
        }

        speedManager.CoalBoostRoutine = StartCoroutine(CoalBoostRoutine(amount));
    }

    private IEnumerator CoalBoostRoutine(float amount)
    {
        float startSpeed = speedManager.CurrentSpeed;
        float targetSpeed = Mathf.Clamp(speedManager.CurrentSpeed + amount, 0f, speedManager.MaxSpeed);

        float timer = 0f;

        while (timer < speedManager.CoalSpeedBoostDuration)
        {
            timer += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(timer / speedManager.CoalSpeedBoostDuration);
            float curveValue = speedManager.CoalBoostCurve.Evaluate(normalizedTime);

            speedManager.CurrentSpeed = Mathf.LerpUnclamped(startSpeed, targetSpeed, curveValue);

            yield return null;
        }

        speedManager.CurrentSpeed = targetSpeed;
        speedManager.CoalBoostRoutine = null;
    }

    public SpeedState GetCurrentSpeedState()
    {
        return speedManager.CurrentSpeedState;
    }
    
    public delegate void OnGameOver();
    public OnGameOver onGameOver;
    
    public delegate void OnWin();
    public OnWin onWin;

}
