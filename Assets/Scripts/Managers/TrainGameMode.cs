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

    private void Awake()
    {
        instance = this;
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
        
        if (SpeedManager.instance != null)
        {
            SpeedManager.instance.StartStartup();
        }
    }
    
    private void Win()
    {
        currentState = LevelFlowState.Win;
    }
    
    private void GameOver()
    {
        currentState = LevelFlowState.GameOver;
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
    
    public delegate void OnGameOver();
    public OnGameOver onGameOver;
    
    public delegate void OnWin();
    public OnWin onWin;

}
