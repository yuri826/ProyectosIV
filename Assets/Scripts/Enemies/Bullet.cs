using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float damage;
    
    private Vector3 moveDirection;
    private GameObject owner;

    private Rigidbody rb;

    public void Init(Vector3 direction, GameObject newOwner)
    {
        moveDirection = direction.normalized;
        owner = newOwner;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.linearVelocity = moveDirection * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        
        if (other.transform.root.gameObject == owner.transform.root.gameObject)
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();

        damageable.TakeDamage(damage);
        
        Destroy(gameObject);
    }
}
