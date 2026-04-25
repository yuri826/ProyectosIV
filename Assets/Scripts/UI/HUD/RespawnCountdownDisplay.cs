using System;
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
        int secondsToShow = Mathf.CeilToInt(remainingTime);
        countdownText.text = secondsToShow.ToString();
        countdownText.gameObject.SetActive(true);

        backgroundObject.SetActive(true);
    }

    public void Hide()
    {
        countdownText.gameObject.SetActive(false);
        backgroundObject.SetActive(false);
    }
}
