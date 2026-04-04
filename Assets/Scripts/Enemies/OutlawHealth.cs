using UnityEngine;

public class OutlawHealth : MonoBehaviour, IDamageable
{
    [Header("Outlaw Health")]
    [SerializeField] private float maxHealth = 3f;

    private float currentHealth;
    private OutlawSystem outlawSystem;

    private void Awake()
    {
        outlawSystem = GetComponent<OutlawSystem>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (outlawSystem != null)
        {
            outlawSystem.OnDead();
        }

        Destroy(gameObject);
    }
}
