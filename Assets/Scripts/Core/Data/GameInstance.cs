using System;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;

    public MapNodeState[] mapNodeStates { get; set; } = new MapNodeState[5];
    public int[] levelScores { get; set; } = new int[5];
    
    private void Awake()
    {
        if (instance is null) instance = this;
        else Destroy(gameObject);

        for (var index = 0; index < mapNodeStates.Length; index++)
        {
            mapNodeStates[index] = MapNodeState.Locked;
        }
        mapNodeStates[0] = MapNodeState.Unlocked;
        
        DontDestroyOnLoad(gameObject);
    }

    public void LevelComplete(int levelIndex, int score)
    {
        int maxLevel = 4;
        
        if (score > levelScores[levelIndex]) levelScores[levelIndex] = score;
        mapNodeStates[levelIndex] = MapNodeState.Completed;
        
        if (levelIndex == maxLevel) return;
        if (mapNodeStates[levelIndex + 1] == MapNodeState.Locked) mapNodeStates[levelIndex + 1] = MapNodeState.Unlocked;
    }
}
