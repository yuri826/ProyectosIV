using System.Collections;
using TMPro;
using UnityEngine;

public class UIUpdater : GamemodeSubsystem
{
    [SerializeField] private TextMeshProUGUI introNumbers;
    [SerializeField] private Animator numberAnim;
    
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
    }
    
    private void ShowIntroText(string textToShow)
    {
        introNumbers.text = textToShow;
        numberAnim.SetTrigger("numberPop");
    }
}
