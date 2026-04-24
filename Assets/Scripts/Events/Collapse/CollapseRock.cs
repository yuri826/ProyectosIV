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
    [SerializeField] private CollapseRockWarning warningShadow;

    [Header("Fall")]
    [SerializeField] private float fallHeight = 6f;
    [SerializeField] private float fallDuration = 0.4f;
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Impact")]
    [SerializeField] private float crushRadius = 1f;
    [SerializeField] private GameObject impactVfxPrefab;
    [SerializeField] private ParticleSystem impactParts;
    
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

        //Si va a estar con el objeto desde el principio no se mete instantiate, mejor que venga ya con el objeto
        //Y se inicialize con él, haciendo ya luego lo que sea
        
        //Aqui en vez de referenciar un objeto y hacer getcomponent, si el objeto luego no se usa para nada hacer
        //referencia directa al componente
        //CollapseRockWarning warning = warningShadow.GetComponent<CollapseRockWarning>();
        
        warningShadow.StartWarning(warningDuration);

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
        impactParts.Play();
        StartCoroutine(LandedRoutine());
    }

    private IEnumerator LandedRoutine()
    {
        hasLanded = true;

        KillObjectsInImpact();
        
        //No instanciar cosas en general mientras se pueda
        //Aquí mejor que las partículas ya nazcan con el objeto, luego se les da al play cuando se necesiten
        //SpawnImpactVfx();

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
