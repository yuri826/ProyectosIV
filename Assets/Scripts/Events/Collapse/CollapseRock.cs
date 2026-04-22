using System.Collections;
using UnityEngine;

public class CollapseRock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PushableObj pushableObj;
    [SerializeField] private Collider pushTrigger;
    [SerializeField] private Rigidbody rb;

    [Header("Damage")]
    [SerializeField] private float damageToTrain = 5f;
    
    [Header("Visual")]
    [SerializeField] private GameObject rockVisual;

    [Header("Warning")]
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private GameObject warningShadowPrefab;

    [Header("Fall")]
    [SerializeField] private float fallHeight = 6f;
    [SerializeField] private float fallDuration = 0.4f;
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Impact")]
    [SerializeField] private float crushRadius = 1f;
    [SerializeField] private GameObject impactVfxPrefab;
    
    [Header("Colliders")]
    [SerializeField] private Collider solidCollider;

    private CollapseRockSpawnPoint currentSpawnPoint;
    private CollapseSystem collapseSystem;

    private bool hasLanded = false;
    private bool hasReturnedTrainLife = false;

    public void StartFall(CollapseRockSpawnPoint spawnPoint, CollapseSystem system)
    {
        currentSpawnPoint = spawnPoint;
        collapseSystem = system;

        currentSpawnPoint.SetOccupied(true);

        transform.position = currentSpawnPoint.transform.position;

        rockVisual.SetActive(false);
        pushableObj.enabled = false;
        pushTrigger.enabled = false;
        solidCollider.enabled = false;

        rb.isKinematic = true;
        rb.useGravity = false;

        StartCoroutine(FallSequence());
    }

    private IEnumerator FallSequence()
    {
        Vector3 landingPosition = currentSpawnPoint.transform.position;
        Vector3 startPosition = landingPosition + Vector3.up * fallHeight;

        GameObject warningShadow = Instantiate(
            warningShadowPrefab,
            landingPosition + Vector3.up * 0.1f,
            Quaternion.identity
        );

        CollapseRockWarning warning = warningShadow.GetComponent<CollapseRockWarning>();
        warning.StartWarning(warningDuration);

        yield return new WaitForSeconds(warningDuration);

        rockVisual.SetActive(true);
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
        StartCoroutine(LandedRoutine());
    }

    private IEnumerator LandedRoutine()
    {
        hasLanded = true;

        KillObjectsInImpact();
        SpawnImpactVfx();

        yield return null;

        solidCollider.enabled = true;
        pushTrigger.enabled = true;
        pushableObj.enabled = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        TrainGameMode.instance.TakeDamage(damageToTrain);
        collapseSystem.OnRockLanded();
    }

    private void KillObjectsInImpact()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, crushRadius);

        TrainCarZone currentCarZone = TrainGameMode.instance.GetCartManager().FindCarZoneForPosition(transform.position);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            PlayerHealthManager playerHealth = hitColliders[i].GetComponentInParent<PlayerHealthManager>();

            if (playerHealth != null)
            {
                playerHealth.KillFromCarZone(currentCarZone);
                continue;
            }

            OutlawHealth outlawHealth = hitColliders[i].GetComponentInParent<OutlawHealth>();

            if (outlawHealth != null)
            {
                outlawHealth.TakeDamage(999f);
            }
        }
    }

    private void SpawnImpactVfx()
    {
        if (impactVfxPrefab == null)
        {
            return;
        }

        Instantiate(impactVfxPrefab, transform.position, Quaternion.identity);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, crushRadius);
    }
}
