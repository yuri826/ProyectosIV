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

    private Animator anim;
    
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
        
        StartCoroutine(SecondCount());
    }

    private IEnumerator SecondCount()
    {
        yield return new WaitForSeconds(1);
        
        Debug.Log(levelTimer);
        
        levelTimer++;
        progressBar.fillAmount = (float)levelTimer / (float)levelDuration;

        if (levelTimer >= levelDuration)
        {
            CompleteLevel();
        }
        else
        {
            StartCoroutine(SecondCount());
        }
    }

    private void CompleteLevel()
    {
        print("Level complete!");
    }
}
