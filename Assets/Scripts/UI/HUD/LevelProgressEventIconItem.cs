using UnityEngine;
using UnityEngine.UI;

public class LevelProgressEventIconItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image eventIconImage;
    public Sprite eventIconSprite { get; set; }

    public void Initialize()
    {
        eventIconImage.sprite = eventIconSprite;
    }
}
