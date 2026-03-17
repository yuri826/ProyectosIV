using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvasMenu : CanvasMenu
{
    [Header("Stars")]
    [SerializeField] private Image[] stars;

    public void StarShow(int amount)
    {
        foreach (Image star in stars)
        {
            star.enabled = false;
        }
        
        StartCoroutine(ShowStars(amount));
    }

    private IEnumerator ShowStars(int amount)
    {
        yield return new WaitForSeconds(0.4f);
        
        for (int i = 0; i < amount; i++)
        {
            stars[i].enabled = true;
            yield return new WaitForSeconds(0.4f);
        }
    }
    
}
