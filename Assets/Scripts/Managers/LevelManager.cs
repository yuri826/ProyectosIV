using UnityEngine;

[CreateAssetMenu(fileName = "LevelManager", menuName = "Scriptable Objects/LevelManager")]
public class LevelManager : ScriptableObject
{
    public int CurrentLevel = 0;
    public int NewLevel = 0;
}
