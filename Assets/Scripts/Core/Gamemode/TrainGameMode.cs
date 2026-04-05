using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrainGameMode : MonoBehaviour
{
    public static TrainGameMode instance;
    
    [Header("Components")]
    [SerializeField] private float introTime;
    [SerializeField] private UIUpdater uiUpdater;
    [SerializeField] private LevelFlow levelFlow;
    [SerializeField] private TrainLife trainLife;
    [SerializeField] private PlayerSubsystem playerSystem;
    [SerializeField] private LevelIntroManager levelIntroManager;
    [SerializeField] private LevelEventSubsystem levelEventManager;

    //Poner en un input manager
    [SerializeField] private InputActionMap gameplayMap;
    
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
        levelIntroManager.TrainGameMode = this;
    
        uiUpdater.OnStart();
        levelFlow.OnStart();
        playerSystem.OnStart();
        trainLife.OnStart();
        levelIntroManager.OnStart();

        onWin += Win;
        onWin += uiUpdater.OnWin;
        onWin += levelFlow.OnWin;
        onWin += playerSystem.EndGameplay;
    
        onGameOver += GameOver;
        onGameOver += uiUpdater.OnGameOver;
        onGameOver += levelFlow.OnGameOver;
        onGameOver += playerSystem.EndGameplay;

        DeactivatePlayers();
    }

    public void Update()
    {
        switch (currentState)
        {
            case LevelFlowState.Intro:
                
                levelIntroManager.OnUpdate();

                break;
            
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
        ActivatePlayers();
        
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

    public void ActivatePlayers()
    {
        foreach (var player in playerSystem.players)
        {
            player?.EnablePlayer();
        }
    }
    
    public void DeactivatePlayers()
    {
        foreach (var player in playerSystem.players)
        {
            player?.DisablePlayer();
        }
    }

    public PlayerInput GetPlayerInput(int playerN)
    {
        return playerSystem.GetPlayer(playerN).playerInput;
    }

    public void SetPlayerInputMap()
    {
        foreach (var player in playerSystem.players)
        {
            player.playerInput.SwitchCurrentActionMap("Gameplay");
        }
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
    
    //EventManager

    public LevelEventSubsystem GetLevelEventManager()
    {
        return levelEventManager;
    }
    
    public delegate void OnGameOver();
    public OnGameOver onGameOver;
    
    public delegate void OnWin();
    public OnWin onWin;

}
