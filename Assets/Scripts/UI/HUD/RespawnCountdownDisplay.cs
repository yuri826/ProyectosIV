using TMPro;
using UnityEngine;

public class RespawnCountdownDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject backgroundObject;

    private void Awake()
    {
        Hide();
    }

    public void ShowTime(float remainingTime)
    {
        if (countdownText == null)
        {
            return;
        }

        int secondsToShow = Mathf.CeilToInt(remainingTime);
        countdownText.text = secondsToShow.ToString();
        countdownText.gameObject.SetActive(true);

        if (backgroundObject != null)
        {
            backgroundObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (backgroundObject != null)
        {
            backgroundObject.SetActive(false);
        }
    }
}
