using System;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float shootSpd;
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
        
    }
}
