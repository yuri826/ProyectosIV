using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressEventIconsHUD : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private RectTransform progressIconsRoot;

    [SerializeField] private LevelEventManager levelEventManager;
    [SerializeField] private TrainSpawnDirector trainSpawnDirector;
    [SerializeField] private SandstormSystem sandstormSystem;

    private readonly List<GameObject> spawnedIcons = new List<GameObject>();

    private IEnumerator Start()
    {
        yield return null;
        BuildIcons();
    }

    public void BuildIcons()
    {
        ClearIcons();

        int levelDuration = TrainGameMode.instance.GetLevelDuration();
        List<LevelEventData> levelEvents = levelEventManager.GetLevelEvents();

        for (int i = 0; i < levelEvents.Count; i++)
        {
            LevelEventData eventData = levelEvents[i];

            if (eventData.iconPrefab == null)
            {
                continue;
            }

            GameObject iconObject = Instantiate(eventData.iconPrefab, progressIconsRoot);
            spawnedIcons.Add(iconObject);

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();

            float normalizedTime = Mathf.Clamp01((float)eventData.triggerTime / levelDuration);
            float width = progressIconsRoot.rect.width;
            float xPos = (normalizedTime - 0.5f) * width;

            iconRect.anchoredPosition = new Vector2(xPos, iconRect.anchoredPosition.y);

            LevelProgressEventIconItem iconItem = iconObject.GetComponent<LevelProgressEventIconItem>();
            iconItem.Initialize(eventData, trainSpawnDirector, sandstormSystem);
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
