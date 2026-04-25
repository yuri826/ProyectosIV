using UnityEngine;

public class OutlawDynamite : MonoBehaviour
{
    [SerializeField] private GameObject explosionVfx;

    private float fuseTime;
    private float damageToTrain;
    private float damageInExplosion;
    private float explosionRadius;
    
    private SabotagePoint targetSabotagePoint;
    
    private bool isInitialized = false;
    
    private void Update()
    {
        if (!isInitialized) return;

        //Cuenta atrás para explotar
        if (fuseTime <= 0f) Explode();
        else fuseTime -= Time.deltaTime;
    }
    
    public void Init(SabotagePoint sabotagePoint, float newFuseTime, float newDamageToTrain, float newDamageInExplosion, float newExplosionRadius)
    {
        // Este método se llama justo después de instanciar la dinamita.
        // Sirve para pasarle todos los datos que necesita esta instancia concreta.
        Debug.Log("Dinamita inicializada");
        
        targetSabotagePoint = sabotagePoint;
        fuseTime = newFuseTime;
        damageToTrain = newDamageToTrain;
        damageInExplosion = newDamageInExplosion;
        explosionRadius = newExplosionRadius;

        isInitialized = true;
    }
    
    private void Explode()
    {
        //Rompe el punto del tren
        targetSabotagePoint.BreakPoint();

        //Hace daño al tren
        float finalDamageToTrain = damageToTrain;
        if (targetSabotagePoint is not null) finalDamageToTrain = targetSabotagePoint.damageAmount;
        TrainGameMode.instance.TakeDamage(finalDamageToTrain);
        
        //Busca a los colliders que puedan recibir daño de la dinamita
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hitCollider in collidersHit)
        {
            if (hitCollider.isTrigger) continue;
            
            hitCollider.SendMessage("TakeDamage", damageInExplosion, SendMessageOptions.DontRequireReceiver);
        }
        
        //Se destruye
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
