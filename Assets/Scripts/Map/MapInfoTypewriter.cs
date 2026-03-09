using System.Collections;
using TMPro;
using UnityEngine;

public class MapInfoTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private float textSpeed;
    
    public void TypeText(string titleText, string bodyText, float secondsTillWrite)
    {
        this.titleText.text = "";
        this.bodyText.text = "";
        
        StartCoroutine(TypeBody(bodyText, secondsTillWrite));
        StartCoroutine(TypeTitle(titleText, secondsTillWrite));
    }

    private IEnumerator TypeBody(string text, float secondsTillWrite)
    {
        yield return new WaitForSeconds(secondsTillWrite);

        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    
    private IEnumerator TypeTitle(string text, float secondsTillWrite)
    {
        //yield return new WaitForSeconds(secondsTillWrite);

        foreach (char c in text)
        {
            titleText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
