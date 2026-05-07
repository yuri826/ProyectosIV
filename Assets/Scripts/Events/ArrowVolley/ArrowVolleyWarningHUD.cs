using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowVolleyWarningHUD : MonoBehaviour
{
    private Image warningIcon;

    private void Awake()
    {
        warningIcon = GetComponent<Image>();
        warningIcon.color = new Color(1, 1, 1, 0); 
    }

    public void ShowWarning()
    {
        StartCoroutine(WarningRoutine());
    }

    public IEnumerator WarningRoutine()
    {
        float a = 1;
        
        for (int i = 0; i < 3; i++)
        {
            while (a > 0)
            {
                warningIcon.color = new Color(1, 1, 1, a); 
                a -= Time.deltaTime;
                yield return null;
            }
        }
        
        warningIcon.color = new Color(1, 1, 1, 0); 
    }
}
