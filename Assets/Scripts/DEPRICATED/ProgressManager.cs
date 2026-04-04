using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    //Deprecated
    
    // [Tooltip("In seconds")][SerializeField] private int levelDuration;
    // private int levelTimer;
    //
    // [SerializeField] private Image progressBar;
    // [SerializeField] private TextMeshProUGUI introNumbers;
    //
    // [Header("Menus")]
    // [SerializeField] private GameObject winMenu;
    // [SerializeField] private GameObject loseMenu;
    //
    // [Header("Events")]
    // [Tooltip("Que corresponda con los tiempos")][SerializeField] private UnityEvent[] spawnEvents;
    // [Tooltip("Que corresponda con los eventos")][SerializeField] private int[] spawnTimes;
    // [Tooltip("Que corresponda con todo lo demás")][SerializeField] private GameObject[] spawnIcons;
    // [SerializeField] private Transform iconParent;
    // [SerializeField] private int iconYOffset;
    //
    // private Animator anim;
    //
    // [SerializeField] private LevelFlowState currentState = LevelFlowState.Intro;
    //
    // private Coroutine countRoutine;
    //
    // private void Start()
    // {
    //     anim = GetComponent<Animator>();
    //     
    //     StartCoroutine(LevelStartSequence());
    //     progressBar.fillAmount = 0;
    // }
    //
    // private IEnumerator LevelStartSequence()
    // {
    //     yield return new WaitForSeconds(1);
    //     introNumbers.text = "3";
    //     anim.SetTrigger("numberPop");
    //     
    //     yield return new WaitForSeconds(1);
    //     introNumbers.text = "2";
    //     anim.SetTrigger("numberPop");
    //     
    //     yield return new WaitForSeconds(1);
    //     introNumbers.text = "1";
    //     anim.SetTrigger("numberPop");
    //     
    //     yield return new WaitForSeconds(1);
    //     introNumbers.text = "GO";
    //     anim.SetTrigger("numberPop");
    //
    //     currentState = LevelFlowState.Gameplay;
    //
    //     PlayerSystem.instance.ActivatePlayers();
    //     
    //     countRoutine = StartCoroutine(SecondCount());
    // }
    //
    // private IEnumerator SecondCount()
    // {
    //     yield return new WaitForSeconds(1);
    //     
    //     levelTimer++;
    //     progressBar.fillAmount = (float)levelTimer / (float)levelDuration;
    //
    //     if (levelTimer >= levelDuration)
    //     {
    //         CompleteLevel();
    //     }
    //     else
    //     {
    //         if (currentState == LevelFlowState.Gameplay) StartCoroutine(SecondCount());
    //
    //         for (var i = 0; i < spawnTimes.Length; i++)
    //         {
    //             if (levelTimer == spawnTimes[i])
    //             {
    //                 spawnEvents[i].Invoke();
    //             }
    //         }
    //     }
    // }
    //
    // private void SetupAnomalyIcons()
    // {
    //     int totalLength = 1400; //-700 : 700
    //
    //     for (var i = 0; i < spawnEvents.Length; i++)
    //     {
    //         float percent = (float)spawnTimes[i] / levelDuration;
    //         int xPos = (int)(totalLength * percent);
    //         RectTransform icon = Instantiate(spawnIcons[i], iconParent).GetComponent<RectTransform>();
    //         icon.position = new Vector2(xPos, icon.position.y + iconYOffset);
    //     }
    // }
    //
    // private void CompleteLevel()
    // {
    //     //Por si acaso se muere justo en el final (improbable pero no imposible :o)
    //     if (currentState != LevelFlowState.Gameplay) return;
    //     
    //     PlayerSystem.instance.DeactivatePlayers(PlayerState.Locked);
    //     winMenu.SetActive(true);
    //    // winMenu.GetComponent<WinCanvasMenu>().StarShow(Ge);
    //
    //     currentState = LevelFlowState.Win;
    //     print("Level complete!");
    // }
    //
    // public void GameOver()
    // {
    //     PlayerSystem.instance.deactivatePlayers();
    //     loseMenu.SetActive(true);
    //     
    //     StopCoroutine(countRoutine);
    //     currentState = LevelFlowState.GameOver;
    // }
}
