using UnityEngine;

public class OutlawBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float damage;
    
    public Vector3 shootDirection;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.linearVelocity = shootDirection * bulletSpeed;
        
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        
        //Queremos fuego amigo? Podria ser interesante entre los enemigos?
        if (other.GetComponent<OutlawHealth>() != null)
        {
            return;
        }

        if (other.TryGetComponent(out OutlawHealth health))
        {
            health.TakeDamage(damage);
        }
        
        // if (other.TryGetComponent(out PlayerHealth playerHealth))
        // {
        //     playerHealth.TakeDamage(damage);
        // }
        
        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
