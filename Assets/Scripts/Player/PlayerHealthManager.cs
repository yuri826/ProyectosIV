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

        TrainCarZone deadCarZone = null;
        TrainCarZone[] trainCarZones = FindObjectsByType<TrainCarZone>(FindObjectsSortMode.None);

        for (int i = 0; i < trainCarZones.Length; i++)
        {
            if (trainCarZones[i] == null)
            {
                continue;
            }

            if (trainCarZones[i].ContainsPoint(transform.position))
            {
                deadCarZone = trainCarZones[i];
                break;
            }
        }

        if (deadCarZone != null)
        {
            OutlawSystem[] outlaws = FindObjectsByType<OutlawSystem>(FindObjectsSortMode.None);

            for (int i = 0; i < outlaws.Length; i++)
            {
                if (outlaws[i] == null)
                {
                    continue;
                }

                if (!deadCarZone.ContainsPoint(outlaws[i].transform.position))
                {
                    continue;
                }

                outlaws[i].OnPlayerFell(playerMovement);
            }
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
