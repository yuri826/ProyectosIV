using System;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float shootSpd;
    [SerializeField] private float damage = 1f;
    public Vector3 shootDir { get; set; }
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.linearVelocity = shootDir * shootSpd;
        Destroy(this.gameObject, 4f);
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
