using UnityEngine;

public class ArrowVolleyWarningHUD : MonoBehaviour
{
    [Header("Warnings")]
    [SerializeField] private GameObject topWarning;
    [SerializeField] private GameObject bottomWarning;
    [SerializeField] private GameObject leftWarning;
    [SerializeField] private GameObject rightWarning;

    private void Awake()
    {
        HideAllWarnings();
    }

    public void ShowWarning(ArrowVolleyDirection direction)
    {
        HideAllWarnings();

        switch (direction)
        {
            case ArrowVolleyDirection.TopToBottom:
                topWarning.SetActive(true);
                break;

            case ArrowVolleyDirection.BottomToTop:
                bottomWarning.SetActive(true);
                break;

            case ArrowVolleyDirection.LeftToRight:
                leftWarning.SetActive(true);
                break;

            case ArrowVolleyDirection.RightToLeft:
                rightWarning.SetActive(true);
                break;
        }
    }

    public void HideWarning(ArrowVolleyDirection direction)
    {
        switch (direction)
        {
            case ArrowVolleyDirection.TopToBottom:
                topWarning.SetActive(false);
                break;

            case ArrowVolleyDirection.BottomToTop:
                bottomWarning.SetActive(false);
                break;

            case ArrowVolleyDirection.LeftToRight:
                leftWarning.SetActive(false);
                break;

            case ArrowVolleyDirection.RightToLeft:
                rightWarning.SetActive(false);
                break;
        }
    }

    public void HideAllWarnings()
    {
        topWarning.SetActive(false);
        bottomWarning.SetActive(false);
        leftWarning.SetActive(false);
        rightWarning.SetActive(false);
    }
}
