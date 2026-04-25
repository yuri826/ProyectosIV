using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIUpdater : GamemodeSubsystem
{
    [Header("IntroUI")]
    [SerializeField] private TextMeshProUGUI introNumbers;
    [SerializeField] private Animator numberAnim;

    [Header("Level Menus")]
    [SerializeField] private LevelMenuManager levelMenuManager;

    [Header("ProgressCanvas")]
    [SerializeField] private Image progressBar;

    [Header("HealthCanvas")]
    [SerializeField] private Image healthBar;
    
    [Header("TransitionAnim")]
    [SerializeField] private Animator transitionAnim;


    public override void OnStart()
    {
        progressBar.fillAmount = 0f;
    }

    //Aparece una cuenta atrás
    public IEnumerator IntroRoutine(float timeBetween)
    {
        yield return new WaitForSeconds(timeBetween);
        ShowIntroText("3");

        yield return new WaitForSeconds(timeBetween);
        ShowIntroText("2");

        yield return new WaitForSeconds(timeBetween);
        ShowIntroText("1");

        yield return new WaitForSeconds(timeBetween);
        ShowIntroText("GO");

        TrainGameMode.StartGameplay();
    }
    
    private void ShowIntroText(string textToShow)
    {
        introNumbers.text = textToShow;
        numberAnim.SetTrigger("numberPop");
    }

    //Updatea el fill de las barras
    public void UpdateProgressBar(int progress, int maxProgress)
    {
        progressBar.fillAmount = (float)progress / maxProgress;
    }

    public void UpdateLifeBar(float currentLife, int maxLife)
    {
        healthBar.fillAmount = currentLife / maxLife;
    }

    //Eventos de ganar, game over y transición
    public override void OnGameOver()
    {
        levelMenuManager.OpenDefeatMenu();
    }

    public override void OnWin()
    {
        levelMenuManager.OpenVictoryMenu();
    }

    public void TransitionIn()
    {
        transitionAnim.SetTrigger("TransitionIn");
    }
}
