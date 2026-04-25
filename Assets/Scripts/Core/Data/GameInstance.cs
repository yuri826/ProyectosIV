using System;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;

    public MapNodeState[] mapNodeStates { get; set; } = new MapNodeState[5];
    public int[] levelScores { get; set; } = new int[5];
    
    private void Awake()
    {
        //Singleton
        if (instance is null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        //Locks every map except the first one
        for (var index = 0; index < mapNodeStates.Length; index++)
        {
            mapNodeStates[index] = MapNodeState.Locked;
        }
        mapNodeStates[0] = MapNodeState.Unlocked;
        
    }

    public void LevelComplete(int levelIndex, int score)
    {
        //Overridea score si es más alto y completa el nivel
        if (score > levelScores[levelIndex]) levelScores[levelIndex] = score;
        mapNodeStates[levelIndex] = MapNodeState.Completed;
        
        //Previene que no se desbloquee un siguiente nivel cuando no hay
        int maxLevel = mapNodeStates.Length-1;
        if (levelIndex == maxLevel) return;
        if (mapNodeStates[levelIndex + 1] == MapNodeState.Locked) mapNodeStates[levelIndex + 1] = MapNodeState.Unlocked;
    }
}
