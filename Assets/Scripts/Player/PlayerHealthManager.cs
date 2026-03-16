using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        playerMovement.state = "gameOver";
        gameObject.SetActive(false);
    }
}
