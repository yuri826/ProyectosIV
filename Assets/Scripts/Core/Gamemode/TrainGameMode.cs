using System;
using UnityEngine;

public class TrainGameMode : MonoBehaviour
{
    public static TrainGameMode instance;
    
    [Header("Components")]
    [SerializeField] private float introTime;
    [SerializeField] private UIUpdater uiUpdater;
    [SerializeField] private LevelFlow levelFlow;
    [SerializeField] private TrainLife trainLife;
    [SerializeField] private PlayerSubsystem playerSystem;
    
    [Header("Gameplay")]
    private LevelFlowState currentState = LevelFlowState.Intro;
    
    [Header("Intro")]
    [SerializeField] private LevelIntroManager levelIntroManager;

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
        trainLife.OnStart();

        onWin += Win;
        onWin += uiUpdater.OnWin;
        onWin += levelFlow.OnWin;
        onWin += playerSystem.EndGameplay;
    
        onGameOver += GameOver;
        onGameOver += uiUpdater.OnGameOver;
        onGameOver += levelFlow.OnGameOver;
        onGameOver += playerSystem.EndGameplay;
        
        levelIntroManager.OpenIntro();
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
        levelIntroManager.SwitchToGameplayInput();
        
        if (SpeedManager.instance != null)
        {
            SpeedManager.instance.StartStartup();
        }
    }
    
    public void StartLevelCountdown()
    {
        StartCoroutine(uiUpdater.IntroRoutine(introTime));
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
    
    public int GetLevelDuration()
    {
        return levelFlow.GetLevelDuration();
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
    
    public delegate void OnGameOver();
    public OnGameOver onGameOver;
    
    public delegate void OnWin();
    public OnWin onWin;

}
