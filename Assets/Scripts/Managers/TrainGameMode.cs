using UnityEngine;

public class TrainGameMode : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private float introTime;
    [SerializeField] private UIUpdater uiUpdater;
    [SerializeField] private LevelFlow levelFlow;
    
    [Header("Gameplay")]

    private LevelFlowState currentState = LevelFlowState.Intro;

    private void Start()
    {
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

    public void Win()
    {
        levelFlow.OnWin();
        uiUpdater.OnWin();
    }
}
