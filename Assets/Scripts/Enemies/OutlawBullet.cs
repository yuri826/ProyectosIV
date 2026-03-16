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

        IDamageable damageable = other.GetComponent<IDamageable>();
        
        damageable.TakeDamage(damage);
        
        Destroy(gameObject);
    }
}
