using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "MapNodeInfo", menuName = "Scriptable Objects/Map Node Info")]
public class MapNodeInfo : ScriptableObject
{
    [SerializeField] private string title;
    [TextArea(2,2)][SerializeField] private string info;
    public int score { get; set; }

    public string Title => title;
    public string Info => info;
}
