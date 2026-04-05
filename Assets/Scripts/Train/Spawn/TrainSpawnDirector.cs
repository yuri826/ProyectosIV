using System.Collections.Generic;
using UnityEngine;

public class TrainSpawnDirector : MonoBehaviour
{
    public static TrainSpawnDirector Instance;
    
    [System.Serializable]
    private class SpawnPointData
    {
        public Transform spawnPoint;
        public TrainCarZone carZone;
    }

    [Header("Train Setup")]
    [SerializeField] private TrainCarZone[] trainCarZones;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject outlawPrefab;

    [Header("Speed Modifiers")]
    [SerializeField] private int lowSpeedOutlawExtra = 4;
    [SerializeField] private int middleSpeedOutlawExtra = 2;
    [SerializeField] private int highSpeedOutlawExtra = 0;

    private readonly List<SpawnPointData> allOutlawSpawnPoints = new List<SpawnPointData>();

    private void Awake()
    {
        //Sería mejor si fuese parte del gamemode pero no puede llamar al Instantiate de abajo
        //Ese método debería de ir en el gamemode
        Instance = this;
        
        CacheAllSpawnPoints();
    }

    private void CacheAllSpawnPoints()
    {
        allOutlawSpawnPoints.Clear();

        for (int i = 0; i < trainCarZones.Length; i++)
        {
            Transform[] spawnPoints = trainCarZones[i].GetSpawnPoints();

            for (int j = 0; j < spawnPoints.Length; j++)
            {
                SpawnPointData newData = new SpawnPointData
                {
                    spawnPoint = spawnPoints[j],
                    carZone = trainCarZones[i]
                };

                allOutlawSpawnPoints.Add(newData);
            }
        }
    }

    public void SpawnOutlawWave(int baseOutlawCount)
    {
        int finalOutlawCount = GetModifiedOutlawCount(baseOutlawCount);

        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < allOutlawSpawnPoints.Count; i++)
        {
            availableIndexes.Add(i);
        }

        int spawnAmount = Mathf.Min(finalOutlawCount, allOutlawSpawnPoints.Count);

        for (int i = 0; i < spawnAmount; i++)
        {
            int randomListIndex = Random.Range(0, availableIndexes.Count);
            int selectedIndex = availableIndexes[randomListIndex];
            availableIndexes.RemoveAt(randomListIndex);

            SpawnPointData selectedSpawn = allOutlawSpawnPoints[selectedIndex];

            GameObject outlawObject = Instantiate(
                outlawPrefab,
                selectedSpawn.spawnPoint.position,
                selectedSpawn.spawnPoint.rotation
            );

            OutlawSystem outlawSystem = outlawObject.GetComponent<OutlawSystem>();
            outlawSystem.SetCurrentCarZone(selectedSpawn.carZone);
        }
    }

    private int GetModifiedOutlawCount(int baseOutlawCount)
    {
        int modifier = 0;

        if (SpeedManager.instance != null)
        {
            switch (SpeedManager.instance.GetCurrentSpeedState())
            {
                case SpeedState.Low:
                    modifier = lowSpeedOutlawExtra;
                    break;

                case SpeedState.Middle:
                    modifier = middleSpeedOutlawExtra;
                    break;

                case SpeedState.High:
                    modifier = highSpeedOutlawExtra;
                    break;
            }
        }

        return Mathf.Max(0, baseOutlawCount + modifier);
    }
    
    public int GetOutlawModifierForCurrentSpeed()
    {
        if (SpeedManager.instance == null)
        {
            return 0;
        }

        switch (SpeedManager.instance.GetCurrentSpeedState())
        {
            case SpeedState.Low:
                return lowSpeedOutlawExtra;

            case SpeedState.Middle:
                return middleSpeedOutlawExtra;

            case SpeedState.High:
                return highSpeedOutlawExtra;
        }

        return 0;
    }
}
