using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelFlowManager : MonoBehaviour
{
    //Deprecated
    
    // [Header("Level Time")]
    // [Tooltip("In seconds")]
    // [SerializeField] private int levelDuration;
    // private int currentLevelTime;
    //
    // [Header("UI")]
    // [SerializeField] private Image progressBar;
    // [SerializeField] private TextMeshProUGUI introNumbers;
    //
    // [Header("Menus")]
    // [SerializeField] private GameObject winMenu;
    // [SerializeField] private GameObject loseMenu;
    //
    // [Header("External Systems")]
    // [SerializeField] private LevelEventManager levelEventManager;
    // [SerializeField] private TrainLifeManager trainLifeManager;
    //
    // [Header("Intro Settings")]
    // [SerializeField] private float introStepDuration = 1f;
    //
    // private Animator anim;
    // [SerializeField] private LevelFlowState currentState = LevelFlowState.Intro;
    //
    // private Coroutine levelTimerRoutine;
    //
    // private void Start()
    // {
    //     anim = GetComponent<Animator>();
    //     progressBar.fillAmount = 0f;
    //     
    //     StartCoroutine(LevelStartSequence());
    // }
    //
    // private IEnumerator LevelStartSequence()
    // {
    //     yield return new WaitForSeconds(introStepDuration);
    //     ShowIntroText("3");
    //
    //     yield return new WaitForSeconds(introStepDuration);
    //     ShowIntroText("2");
    //
    //     yield return new WaitForSeconds(introStepDuration);
    //     ShowIntroText("1");
    //
    //     yield return new WaitForSeconds(introStepDuration);
    //     ShowIntroText("GO");
    //
    //     StartGameplay();
    // }
    //
    // private void ShowIntroText(string textToShow)
    // {
    //     introNumbers.text = textToShow;
    //     anim.SetTrigger("numberPop");
    // }
    //
    // private void StartGameplay()
    // {
    //     currentState = LevelFlowState.Gameplay;
    //     PlayerSystem.instance.ActivatePlayers();
    //     levelTimerRoutine = StartCoroutine(LevelTimerLoop());
    // }
    //
    // private IEnumerator LevelTimerLoop()
    // {
    //     while (currentState == LevelFlowState.Gameplay)
    //     {
    //         yield return new WaitForSeconds(1f);
    //
    //         currentLevelTime++;
    //
    //         UpdateProgressBar();
    //         NotifyEventManager();
    //
    //         if (currentLevelTime >= levelDuration)
    //         {
    //             CompleteLevel();
    //             yield break;
    //         }
    //     }
    // }
    //
    // private void UpdateProgressBar()
    // {
    //     progressBar.fillAmount = (float)currentLevelTime / (float)levelDuration;
    // }
    //
    // private void NotifyEventManager()
    // {
    //     levelEventManager.UpdateEventTimeline(currentLevelTime);
    // }
    //
    // public void CompleteLevel()
    // {
    //     if (currentState != LevelFlowState.Gameplay)
    //     {
    //         return;
    //     }
    //
    //     currentState = LevelFlowState.Win;
    //     PlayerSystem.instance.DeactivatePlayers(PlayerState.Locked);
    //     
    //     winMenu.SetActive(true);
    //     winMenu.GetComponent<WinCanvasMenu>().StarShow(CalculateScore());
    //     
    //     StopLevelTimerRoutine();
    // }
    //
    // private int CalculateScore()
    // {
    //     int stars = 0;
    //     
    //     float starPercent3 = 0.9f;
    //     float starPercent2 = 0.7f;
    //     float starPercent1 = 0.5f;
    //
    //     if (trainLifeManager.CurrentTrainLife > trainLifeManager.MaxTrainLife * starPercent3)
    //     {
    //         stars = 3;
    //         goto RETURN;
    //     }
    //     
    //     if (trainLifeManager.CurrentTrainLife > trainLifeManager.MaxTrainLife * starPercent2)
    //     {
    //         stars = 2;
    //         goto RETURN;
    //     }
    //     
    //     if (trainLifeManager.CurrentTrainLife > trainLifeManager.MaxTrainLife * starPercent1)
    //     {
    //         stars = 1;
    //         goto RETURN;
    //     }
    //     
    //     RETURN:
    //
    //     return stars;
    // }
    //
    // public void GameOver()
    // {
    //     currentState = LevelFlowState.GameOver;
    //     PlayerSystem.instance.DeactivatePlayers(PlayerState.GameOver);
    //     loseMenu.SetActive(true);
    //     StopLevelTimerRoutine();
    // }
    //
    // private void StopLevelTimerRoutine()
    // {
    //     StopCoroutine(levelTimerRoutine);
    //     levelTimerRoutine = null;
    // }
    //
    // public int GetCurrentLevelTime()
    // {
    //     return currentLevelTime;
    // }
    //
    // public int GetLevelDuration()
    // {
    //     return levelDuration;
    // }
    //
    // public LevelFlowState GetCurrentState()
    // {
    //     return currentState;
    // }
}
