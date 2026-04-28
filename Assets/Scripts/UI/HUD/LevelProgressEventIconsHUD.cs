using System.Collections.Generic;
using UnityEngine;

public class LevelProgressEventIconsHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform progressIconsRoot;
    [SerializeField] private LevelProgressEventIconItem iconPrefab;

    [Header("Layout")]
    [SerializeField] private float progressBarWidth = 500f;

    private readonly List<LevelProgressEventIconItem> spawnedIcons = new();

    private void Start()
    {
        BuildIcons();
    }

    public void BuildIcons()
    {
        ClearIcons();

        int levelDuration = TrainGameMode.instance.levelFlow.levelDuration;
        LevelEventInfo[] levelEvents = TrainGameMode.instance.GetLevelEventSubsystem().levelEvents;

        foreach (LevelEventInfo eventInfo in levelEvents)
        {
            LevelProgressEventIconItem iconItem = Instantiate(iconPrefab, progressIconsRoot);
            spawnedIcons.Add(iconItem);

            RectTransform iconRect = iconItem.GetComponent<RectTransform>();

            float normalizedTime = Mathf.Clamp01((float)eventInfo.execTime / levelDuration);
            float xPosition = normalizedTime * progressBarWidth;

            iconRect.anchoredPosition = new Vector2(xPosition, iconRect.anchoredPosition.y);

            iconItem.Initialize(eventInfo.eventIcon);
        }
    }

    private void ClearIcons()
    {
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            Destroy(spawnedIcons[i].gameObject);
        }

        spawnedIcons.Clear();
    }
}
