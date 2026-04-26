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
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private RectTransform progressIcon;
    [SerializeField] private float progressBarMaxWidth;
    [SerializeField] private float progressIconStartX;

    [Header("HealthCanvas")]
    [SerializeField] private Image healthBar;
    
    [Header("TransitionAnim")]
    [SerializeField] private Animator transitionAnim;


    public override void OnStart()
    {
        SetProgressHUD(0f);
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

    //Updatea el width de la barra y el icono
    public void UpdateProgressBar(int progress, int maxProgress)
    {
        float progressAmount = Mathf.Clamp01((float)progress / maxProgress);
        SetProgressHUD(progressAmount);
    }

    private void SetProgressHUD(float progressAmount)
    {
        progressBar.sizeDelta = new Vector2(
            progressBarMaxWidth * progressAmount,
            progressBar.sizeDelta.y
        );

        progressIcon.anchoredPosition = new Vector2(
            progressIconStartX + progressBarMaxWidth * progressAmount,
            progressIcon.anchoredPosition.y
        );
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
