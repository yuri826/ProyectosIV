using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private Vector3 shootDirection;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = shootDirection * speed;
    }

    public void Init(Vector3 direction, GameObject newOwner)
    {
        shootDirection = direction.normalized;
        owner = newOwner;

        IgnoreOwnerCollisions();
    }

    private void IgnoreOwnerCollisions()
    {
        if (owner == null)
        {
            return;
        }

        Collider bulletCollider = GetComponent<Collider>();

        if (bulletCollider == null)
        {
            return;
        }

        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();

        for (int i = 0; i < ownerColliders.Length; i++)
        {
            if (ownerColliders[i] == null)
            {
                continue;
            }

            Physics.IgnoreCollision(bulletCollider, ownerColliders[i], true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.transform.root.gameObject == owner)
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        
        if (other.isTrigger && damageable == null)
        {
            return;
        }

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
