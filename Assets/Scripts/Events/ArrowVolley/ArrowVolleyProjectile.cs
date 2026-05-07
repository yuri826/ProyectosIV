using System.Collections;
using UnityEngine;

public class ArrowVolleyProjectile : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float speed = 10f;
    [field : SerializeField] public float damage { get; set; } = 1f;
    [SerializeField] private float lifeTime = 5f;
    
    public Transform poolPos { get; set; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 direction, Vector3 shootPos)
    {
        print(shootPos);
        transform.position = shootPos;
        print(transform.position);
        
        rb.linearVelocity = direction.normalized * speed;
        
        StartCoroutine(DestroyBullet());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damageable)) damageable.TakeDamage(damage);

        GoToPool();
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTime);
        GoToPool();
    }

    private void GoToPool()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = poolPos.position;
    }
}