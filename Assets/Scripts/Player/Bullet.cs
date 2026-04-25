using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float speed = 10f;
    [field : SerializeField] public float damage { get; set; } = 1f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private Collider bulletCollider;
    private Vector3 shootDirection;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        rb.linearVelocity = shootDirection * speed;
    }

    //Podría in en el start y no pasaría nada, si no cambia no hace falta que esté en update
    // private void FixedUpdate()
    // {
    //     rb.linearVelocity = shootDirection * speed;
    // }

    public void Init(Vector3 direction, GameObject newOwner)
    {
        shootDirection = direction.normalized;
        owner = newOwner;

        IgnoreOwnerCollisions();
    }

    private void IgnoreOwnerCollisions()
    {
        if (owner is null) return;

        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();

        foreach (var col in ownerColliders)
        {
            Physics.IgnoreCollision(bulletCollider, col, true);
        }
    }

    //cambiado a onCollision porque después salías del método si era trigger
    private void OnCollisionEnter(Collision other)
    {
        if (owner is not null && other.transform.root.gameObject == owner) return;

        // IDamageable damageable = other.GetComponent<IDamageable>();
        //
        // if (other.isTrigger && damageable == null)
        // {
        //     return;
        // }
        //
        // damageable?.TakeDamage(damage);
        
        //creo que así bastante mejor
        if (other.gameObject.TryGetComponent(out IDamageable damageable)) damageable.TakeDamage(damage);

        Destroy(gameObject);
    }
}
