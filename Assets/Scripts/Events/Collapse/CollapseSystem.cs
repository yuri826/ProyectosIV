using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseSystem : MonoBehaviour
{
    public static CollapseSystem Instance;

    [Header("State")]
    [SerializeField] private bool isCollapseActive = false;
    [SerializeField] private float remainingDuration = 0f;

    [Header("References")]
    [SerializeField] private CollapseRock rockPrefab;
    [SerializeField] private TrainCarZone[] trainCarZones;
    [SerializeField] private ParticleSystem[] collapseParticles;
    [SerializeField] private LevelCamera levelCamera;

    [Header("Camera Shake")]
    [SerializeField] private float collapseShakeAmount = 0.08f;
    [SerializeField] private float rockLandingShakeAmount = 0.18f;

    private Coroutine collapseRoutine;
    private readonly List<CollapseRockSpawnPoint> allSpawnPoints = new List<CollapseRockSpawnPoint>();

    private void Awake()
    {
        Instance = this;
        CacheSpawnPoints();
    }

    private void Update()
    {
        if (!isCollapseActive)
        {
            return;
        }

        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0f)
        {
            StopCollapse();
        }
    }

    public void StartCollapse(float duration, int rockCount)
    {
        if (collapseRoutine != null)
        {
            StopCoroutine(collapseRoutine);
        }

        remainingDuration = duration;
        isCollapseActive = true;

        for (int i = 0; i < collapseParticles.Length; i++)
        {
            collapseParticles[i].Play();
        }
        
        levelCamera.SetCollapseShake(collapseShakeAmount);

        collapseRoutine = StartCoroutine(CollapseRoutine(duration, rockCount));
    }

    public void StopCollapse()
    {
        isCollapseActive = false;
        remainingDuration = 0f;

        for (int i = 0; i < collapseParticles.Length; i++)
        {
            collapseParticles[i].Stop();
        }
        
        levelCamera.SetCollapseShake(0f);

        if (collapseRoutine != null)
        {
            StopCoroutine(collapseRoutine);
            collapseRoutine = null;
        }
    }

    public void OnRockLanded()
    {
        levelCamera.AddImpactShake(rockLandingShakeAmount);
    }

    private void CacheSpawnPoints()
    {
        allSpawnPoints.Clear();

        for (int i = 0; i < trainCarZones.Length; i++)
        {
            CollapseRockSpawnPoint[] carSpawnPoints = trainCarZones[i].GetCollapseSpawnPoints();

            for (int j = 0; j < carSpawnPoints.Length; j++)
            {
                allSpawnPoints.Add(carSpawnPoints[j]);
            }
        }
    }

    private IEnumerator CollapseRoutine(float duration, int rockCount)
    {
        List<float> spawnTimes = BuildSpawnTimes(duration, rockCount);

        for (int i = 0; i < spawnTimes.Count; i++)
        {
            yield return new WaitForSeconds(spawnTimes[i]);
            SpawnRock();
        }

        collapseRoutine = null;
    }

    private List<float> BuildSpawnTimes(float duration, int rockCount)
    {
        List<float> delays = new List<float>();

        if (rockCount <= 0)
        {
            return delays;
        }

        for (int i = 0; i < rockCount; i++)
        {
            delays.Add(Random.Range(0f, duration));
        }

        delays.Sort();

        for (int i = delays.Count - 1; i > 0; i--)
        {
            delays[i] -= delays[i - 1];
        }

        return delays;
    }

    private void SpawnRock()
    {
        List<CollapseRockSpawnPoint> freePoints = GetFreeSpawnPoints();

        if (freePoints.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, freePoints.Count);
        CollapseRockSpawnPoint selectedPoint = freePoints[randomIndex];

        CollapseRock rock = Instantiate(rockPrefab);
        rock.StartFall(selectedPoint, this);
    }

    private List<CollapseRockSpawnPoint> GetFreeSpawnPoints()
    {
        List<CollapseRockSpawnPoint> freePoints = new List<CollapseRockSpawnPoint>();

        for (int i = 0; i < allSpawnPoints.Count; i++)
        {
            if (!allSpawnPoints[i].IsOccupied())
            {
                freePoints.Add(allSpawnPoints[i]);
            }
        }

        return freePoints;
    }
}
