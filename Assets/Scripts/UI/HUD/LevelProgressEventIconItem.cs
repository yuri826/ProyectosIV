using UnityEngine;
using UnityEngine.UI;

public class LevelProgressEventIconItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image eventIconImage;

    public void Initialize(Sprite eventIcon)
    {
        eventIconImage.sprite = eventIcon;
    }
}
