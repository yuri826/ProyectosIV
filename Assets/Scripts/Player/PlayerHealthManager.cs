using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerWeapon playerWeapon;
    [SerializeField] private CharacterController characterController;

    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageCooldown = 1f;

    private float currentHealth;
    private bool isDead = false;
    private bool canTakeDamage = true;
    private Coroutine damageCooldownRoutine;

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if (playerWeapon == null)
        {
            playerWeapon = GetComponent<PlayerWeapon>();
        }

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        if (!canTakeDamage)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (damageCooldownRoutine != null)
        {
            StopCoroutine(damageCooldownRoutine);
        }

        damageCooldownRoutine = StartCoroutine(DamageCooldownRoutine());
    }

    private IEnumerator DamageCooldownRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
        damageCooldownRoutine = null;
    }

    private void Die()
    {
        isDead = true;
        canTakeDamage = false;

        if (damageCooldownRoutine != null)
        {
            StopCoroutine(damageCooldownRoutine);
            damageCooldownRoutine = null;
        }

        if (playerWeapon != null)
        {
            playerWeapon.CancelReload();
        }

        if (playerMovement != null)
        {
            playerMovement.currentState = PlayerState.Locked;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        gameObject.SetActive(false);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
