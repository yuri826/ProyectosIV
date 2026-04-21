using System.Collections;
using UnityEngine;

public class CollapseRock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PushableObj pushableObj;
    [SerializeField] private Collider pushTrigger;

    [Header("Damage")]
    [SerializeField] private float damageToTrain = 5f;

    [Header("Fall")]
    [SerializeField] private float fallHeight = 6f;
    [SerializeField] private float fallDuration = 0.4f;
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CollapseRockSpawnPoint currentSpawnPoint;
    private CollapseSystem collapseSystem;

    private bool hasLanded = false;
    private bool hasReturnedTrainLife = false;

    public void StartFall(CollapseRockSpawnPoint spawnPoint, CollapseSystem system)
    {
        currentSpawnPoint = spawnPoint;
        collapseSystem = system;

        currentSpawnPoint.SetOccupied(true);

        pushableObj.enabled = false;
        pushTrigger.enabled = false;

        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        Vector3 landingPosition = currentSpawnPoint.transform.position;
        Vector3 startPosition = landingPosition + Vector3.up * fallHeight;

        transform.position = startPosition;

        float timer = 0f;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fallDuration);
            float curveT = fallCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPosition, landingPosition, curveT);

            yield return null;
        }

        transform.position = landingPosition;
        OnRockLanded();
    }

    private void OnRockLanded()
    {
        hasLanded = true;

        pushTrigger.enabled = true;
        pushableObj.enabled = true;

        TrainGameMode.instance.TakeDamage(damageToTrain);
        collapseSystem.OnRockLanded();
    }

    public void RemoveRock()
    {
        if (!hasLanded)
        {
            return;
        }

        if (hasReturnedTrainLife)
        {
            return;
        }

        hasReturnedTrainLife = true;

        TrainGameMode.instance.RepairTrain(damageToTrain);
        currentSpawnPoint.SetOccupied(false);

        Destroy(gameObject);
    }
}
