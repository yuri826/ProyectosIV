using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerWeapon playerWeapon;

    [Header("Respawn")]
    [SerializeField] private PlayerRespawnManager playerRespawnManager;
    [SerializeField] private float respawnInvulnerabilityTime = 1.5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageCooldown = 1f;

    private float currentHealth;
    private bool isDead = false;
    private bool canTakeDamage = true;
    private Coroutine damageCooldownRoutine;

    private bool isInvulnerable = false;
    private Coroutine invulnerabilityRoutine;

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

        if (isInvulnerable)
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

    public void KillFromCarZone(TrainCarZone sourceCarZone)
    {
        if (isDead)
        {
            return;
        }

        ForceDie(sourceCarZone);
    }

    private IEnumerator DamageCooldownRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
        damageCooldownRoutine = null;
    }

    public void StartInvulnerability()
    {
        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
        }

        invulnerabilityRoutine = StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(respawnInvulnerabilityTime);

        isInvulnerable = false;
        invulnerabilityRoutine = null;
    }

    private void ForceDie(TrainCarZone forcedCarZone)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        canTakeDamage = false;

        if (damageCooldownRoutine != null)
        {
            StopCoroutine(damageCooldownRoutine);
            damageCooldownRoutine = null;
        }

        playerWeapon.CancelReload();
        playerMovement.currentState = PlayerState.Locked;
        playerRespawnManager.HandleDeath(this, forcedCarZone);
    }

    private void Die()
    {
        ForceDie(null);
    }

    public void ReviveToFullHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        canTakeDamage = true;

        if (damageCooldownRoutine != null)
        {
            StopCoroutine(damageCooldownRoutine);
            damageCooldownRoutine = null;
        }
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
