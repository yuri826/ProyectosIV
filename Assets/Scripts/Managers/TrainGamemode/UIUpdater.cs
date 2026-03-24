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
    
    [Header("EndCanvases")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private WinCanvasMenu winCanvasMenu;
    
    [Header("ProgressCanvas")]
    [SerializeField] private Image progressBar;
    
    [Header("ProgressCanvas")]
    [SerializeField] private Image healthBar;

    public override void OnStart()
    {
        progressBar.fillAmount = 0;
    }

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

    public void UpdateProgressBar(int progress, int maxProgress)
    {
        progressBar.fillAmount = (float)progress / maxProgress;
    }

    public void UpdateLifeBar(float currentLife, int maxLife)
    {
        healthBar.fillAmount = currentLife / maxLife;
    }

    private void ShowIntroText(string textToShow)
    {
        introNumbers.text = textToShow;
        numberAnim.SetTrigger("numberPop");
    }

    public override void OnGameOver()
    {
        gameOverCanvas.SetActive(true);
    }

    public override void OnWin()
    {
        winCanvas.SetActive(true);
        winCanvasMenu.StarShow(3);
    }
}
