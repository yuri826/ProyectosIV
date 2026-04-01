using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    private PlayerMovement playerMovement;
    
    [SerializeField] private float maxHealth = 3f;
    private float currentHealth;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        playerMovement.currentState = PlayerState.GameOver;
        gameObject.SetActive(false);
    }
}
