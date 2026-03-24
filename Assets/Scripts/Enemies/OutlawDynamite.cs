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
        if (!isInitialized)
        {
            return;
        }
        
        fuseTime -= Time.deltaTime;
        Debug.Log("Tiempo restante dinamita: " + fuseTime);
        if (fuseTime <= 0f)
        {
            Explode();
        }
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
        Debug.Log("La dinamita ha explotado", this.gameObject);
        targetSabotagePoint.BreakPoint();
        
        TrainGameMode.instance.TakeDamage(damageToTrain);
        
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < collidersHit.Length; i++)
        {
            Collider hitCollider = collidersHit[i];
            
            if (hitCollider.isTrigger)
            {
                continue;
            }
            
            hitCollider.SendMessage("TakeDamage", damageInExplosion, SendMessageOptions.DontRequireReceiver);
        }
        
        //Instantiate(explosionVfx, transform.position, Quaternion.identity);
        Debug.Log("Destruyendo dinamita");
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
