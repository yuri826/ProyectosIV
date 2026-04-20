using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressEventIconItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image eventIconImage;
    [SerializeField] private TextMeshProUGUI baseValueText;
    [SerializeField] private TextMeshProUGUI modifierText;

    private float baseDuration;

    public void Initialize(LevelEventData eventData)
    {
        baseDuration = eventData.duration;


        SetupBaseText();
        UpdateModifierText();
    }

    private void Update()
    {
        UpdateModifierText();
    }

    protected virtual void SetupBaseText()
    {
        // switch (eventType)
        // {
        //     case LevelEventType.OutlawWave:
        //         baseValueText.text = baseOutlawCount.ToString();
        //         break;
        //
        //     case LevelEventType.Sandstorm:
        //         baseValueText.text = $"{Mathf.RoundToInt(baseDuration)}s";
        //         break;
        // }
    }

    protected virtual void UpdateModifierText()
    {
        // switch (eventType)
        // {
        //     case LevelEventType.OutlawWave:
        //         int outlawModifier = trainSpawnDirector.GetOutlawModifierForCurrentSpeed();
        //         modifierText.text = outlawModifier >= 0 ? $"+{outlawModifier}" : outlawModifier.ToString();
        //         break;
        //
        //     case LevelEventType.Sandstorm:
        //         float stormModifier = sandstormSystem.GetStormModifierForCurrentSpeed();
        //         float percent = (stormModifier - 1f) * 100f;
        //
        //         if (Mathf.Abs(percent) < 0.01f)
        //         {
        //             modifierText.text = "Base";
        //         }
        //         else
        //         {
        //             int roundedPercent = Mathf.RoundToInt(percent);
        //             modifierText.text = roundedPercent > 0 ? $"+{roundedPercent}%" : $"{roundedPercent}%";
        //         }
        //         break;
        // }
    }
}
