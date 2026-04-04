using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvasMenu : CanvasMenu
{
    [Header("Stars")]
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite emptyStarSprite;
    [SerializeField] private Sprite filledStarSprite;
    [SerializeField] private float delayBeforeStart = 0.2f;
    [SerializeField] private float timeBetweenStars = 0.25f;

    private Coroutine starRoutine;

    public void StarShow(int amount)
    {
        amount = Mathf.Clamp(amount, 0, stars.Length);

        if (starRoutine != null)
        {
            StopCoroutine(starRoutine);
        }

        SetupEmptyStars();
        starRoutine = StartCoroutine(ShowStars(amount));
    }

    public void ResetStars()
    {
        if (starRoutine != null)
        {
            StopCoroutine(starRoutine);
            starRoutine = null;
        }

        SetupEmptyStars();
    }

    private void SetupEmptyStars()
    {
        if (stars == null || stars.Length == 0)
        {
            return;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null)
            {
                continue;
            }

            stars[i].enabled = true;

            if (emptyStarSprite != null)
            {
                stars[i].sprite = emptyStarSprite;
            }
        }
    }

    private IEnumerator ShowStars(int amount)
    {
        yield return new WaitForSecondsRealtime(delayBeforeStart);

        for (int i = 0; i < amount; i++)
        {
            if (stars[i] != null && filledStarSprite != null)
            {
                stars[i].sprite = filledStarSprite;
            }

            yield return new WaitForSecondsRealtime(timeBetweenStars);
        }

        starRoutine = null;
    }
}
