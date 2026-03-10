using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    [Tooltip("In seconds")][SerializeField] private int levelDuration;
    private int levelTimer;
    
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI introNumbers;
    
    [Header("Menus")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject loseMenu;

    private Animator anim;

    private string state = "intro";

    private Coroutine countRoutine;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        
        StartCoroutine(LevelStartSequence());
        progressBar.fillAmount = 0;
    }

    private IEnumerator LevelStartSequence()
    {
        yield return new WaitForSeconds(1);
        introNumbers.text = "3";
        anim.SetTrigger("numberPop");
        
        yield return new WaitForSeconds(1);
        introNumbers.text = "2";
        anim.SetTrigger("numberPop");
        
        yield return new WaitForSeconds(1);
        introNumbers.text = "1";
        anim.SetTrigger("numberPop");
        
        yield return new WaitForSeconds(1);
        introNumbers.text = "GO";
        anim.SetTrigger("numberPop");

        state = "gameplay";

        PlayerSystem.instance.ActivatePlayers();
        
        countRoutine = StartCoroutine(SecondCount());
    }

    private IEnumerator SecondCount()
    {
        yield return new WaitForSeconds(1);
        
        levelTimer++;
        progressBar.fillAmount = (float)levelTimer / (float)levelDuration;

        if (levelTimer >= levelDuration)
        {
            CompleteLevel();
        }
        else
        {
            if (state == "gameplay") StartCoroutine(SecondCount());
        }
    }

    private void CompleteLevel()
    {
        //Por si acaso se muere justo en el final (improbable pero no imposible :o)
        if (state != "gameplay") return;
        
        PlayerSystem.instance.DeactivatePlayers("win");
        winMenu.SetActive(true);
        
        state = "win";
        print("Level complete!");
    }

    public void GameOver()
    {
        PlayerSystem.instance.DeactivatePlayers("gameOver");
        loseMenu.SetActive(true);
        
        StopCoroutine(countRoutine);
        state = "gameover";
    }
}
