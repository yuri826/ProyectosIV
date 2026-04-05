using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressEventIconsHUD : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private RectTransform progressIconsRoot;

    private LevelEventSubsystem levelEventManager => TrainGameMode.instance.GetLevelEventManager();

    private readonly List<GameObject> spawnedIcons = new List<GameObject>();

    private IEnumerator Start()
    {
        yield return null;
        BuildIcons();
    }

    public void BuildIcons()
    {
        ClearIcons();

        int maxXPos = 740 * 2; //Calculado a mano

        int levelDuration = TrainGameMode.instance.GetLevelDuration();
        List<LevelEventInstance> levelEvents = levelEventManager.GetLevelEvents();

        foreach (var eventData in levelEvents)
        {
            GameObject iconObject = Instantiate(eventData.eventData.iconPrefab, progressIconsRoot);
            spawnedIcons.Add(iconObject);

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();

            float normalizedTime = Mathf.Clamp01((float)eventData.timeToSpawn / levelDuration);
            float xPos = normalizedTime * maxXPos;

            iconRect.anchoredPosition = new Vector2(xPos, iconRect.anchoredPosition.y);

            LevelProgressEventIconItem iconItem = iconObject.GetComponent<LevelProgressEventIconItem>();
            iconItem.Initialize(eventData.eventData);
        }
    }

    private void ClearIcons()
    {
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            Destroy(spawnedIcons[i]);
        }

        spawnedIcons.Clear();
    }
}
