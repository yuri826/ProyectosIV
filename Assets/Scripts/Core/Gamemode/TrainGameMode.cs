using System;
using UnityEngine;

public class TrainGameMode : MonoBehaviour
{
    public static TrainGameMode instance;

    [SerializeField] private int levelIndex;
    
    [Header("Components")]
    [SerializeField] private float introTime;
    [SerializeField] private UIUpdater uiUpdater;
    [SerializeField] private LevelFlow levelFlow;
    [SerializeField] private TrainLife trainLife;
    [SerializeField] private PlayerSubsystem playerSystem;
    [SerializeField] private CartEnemyManager cartManager;
    [SerializeField] private LevelEventSubsystem levelEventSubsystem;
    [SerializeField] private TrainSpawnDirector trainSpawnDirector;
    [SerializeField] private SpeedManager speedManager;
    
    [Header("Gameplay")]
    private LevelFlowState currentState = LevelFlowState.Intro;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        //Los subsistemas toman referencia al gamemode
        uiUpdater.TrainGameMode = this;
        levelFlow.TrainGameMode = this;
        playerSystem.TrainGameMode = this;
        trainLife.TrainGameMode = this;
    
        //Inicializa los subsistemas
        uiUpdater.OnStart();
        levelFlow.OnStart();
        playerSystem.OnStart();
        trainLife.OnStart();
        speedManager.OnStart();
        speedManager.StartStartup();

        //Suscribe todos los subsistemas necesarios a ganar y perder
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
            case LevelFlowState.Intro:

                speedManager.OnUpdate();

                break;

            case LevelFlowState.Gameplay:
                
                uiUpdater.OnUpdate();
                levelFlow.OnUpdate();
                speedManager.OnUpdate();
                
                break;
        }
    }

    #region GameState
    
    public void StartGameplay()
    {
        currentState = LevelFlowState.Gameplay;
        playerSystem.activatePlayers();
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
    
    #endregion

    #region UI
    
    public void UpdateProgressBar(int progress, int maxProgress)
    {
        uiUpdater.UpdateProgressBar(progress, maxProgress);
    }
    
    public void UpdateLifeBar(float currentLife, int maxLife)
    {
        uiUpdater.UpdateLifeBar(currentLife, maxLife);
    }

    public void TransitionIn()
    {
        uiUpdater.TransitionIn();
    }
    
    #endregion

    #region PlayerSystem
    
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
    
    #endregion
    
    #region Health
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
        return trainLife.currentTrainLife;
    }

    public int GetMaxTrainLife()
    {
        return trainLife.maxTrainLife;
    }
    
    #endregion
    
    #region CartManager
    
    public CartEnemyManager GetCartManager()
    {
        return cartManager;
    }
    
    #endregion
    
    #region Level event subsystem
    
    public LevelEventSubsystem GetLevelEventSubsystem()
    {
        return levelEventSubsystem;
    }
    
    #endregion
    
    #region TrainSpawnDirector
    
    public TrainSpawnDirector GetTrainSpawnDirector()
    {
        return trainSpawnDirector;
    }
    
    #endregion
    
    #region SpeedManager
    
    public SpeedManager GetSpeedManager()
    {
        return speedManager;
    }
    
    #endregion
    
    #region Level
    
    public int GetLevelIndex()
    {
        return levelIndex;
    }
    
    public delegate void OnGameOver();
    public OnGameOver onGameOver;
    
    public delegate void OnWin();
    public OnWin onWin;
    
    #endregion
}
