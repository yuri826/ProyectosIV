using System.Collections;
using TMPro;
using UnityEngine;

public class MapInfoTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private float textSpeed;
    
    private Coroutine titleWrite;
    private Coroutine bodyWrite;
    
    public void TypeText(string titleText, string bodyText, float secondsTillWrite)
    {
        this.titleText.text = "";
        this.bodyText.text = "";
        
        bodyWrite = StartCoroutine(TypeBody(bodyText, secondsTillWrite));
        titleWrite = StartCoroutine(TypeTitle(titleText, secondsTillWrite));
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

    public void StopTyping()
    {
        StopCoroutine(titleWrite);
        titleText.text = "";
        StopCoroutine(bodyWrite);
        bodyText.text = "";
    }
}
