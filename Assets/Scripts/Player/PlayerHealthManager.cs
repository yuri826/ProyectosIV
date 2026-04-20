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
    
    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 5f;
    
    [Header("Camera")]
    [SerializeField] private LevelCamera levelCamera;
    [SerializeField] private float respawnCameraSmoothSpeed = 1f;

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

        playerMovement.ForceDropObj();

        playerWeapon.CancelReload();
        playerMovement.currentState = PlayerState.Locked;
        //playerRespawnManager.HandleDeath(this,playerMovement,playerMovement.GetComponent<CharacterController>(), forcedCarZone);
        
        TrainCarZone deadCarZone = forcedCarZone ?? TrainGameMode.instance.GetCartManager().
            FindCarZoneForPosition(gameObject.transform.position);
    
        if (deadCarZone is not null)
        {
            Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
    
            levelCamera.SetOverrideTarget(respawnPoint, respawnCameraSmoothSpeed);
        }
    
        TrainGameMode.instance.GetTrainSpawnDirector().NotifyOutlawsInDeadCar(deadCarZone, playerMovement);
    
        StartCoroutine(RespawnRoutine(this, playerMovement, 
            playerMovement.GetComponent<CharacterController>(), deadCarZone));
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
    
    private IEnumerator RespawnRoutine(PlayerHealthManager deadPlayerHealth, PlayerMovement deadPlayerMovement, 
        CharacterController deadCharacterController, TrainCarZone deadCarZone)
    {
        GameObject deadPlayer = deadPlayerHealth.gameObject;
    
        deadPlayerMovement.currentState = PlayerState.Locked;
        deadCharacterController.enabled = false;
        deadPlayer.SetActive(false);
    
        RespawnCountdownDisplay countdownDisplay = null;
    
        if (deadCarZone is not null)
        {
            Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
    
            countdownDisplay = respawnPoint.GetComponent<RespawnCountdownDisplay>();
        }
    
        float remainingRespawnTime = respawnDelay;
    
        while (remainingRespawnTime > 0f)
        {
            //Puesto como ? porque maybe puede petar por nulo
            countdownDisplay?.ShowTime(remainingRespawnTime);
    
            yield return null;
            remainingRespawnTime -= Time.deltaTime;
        }
    
        //Puesto como ? porque maybe puede petar por nulo
        countdownDisplay?.Hide();
    
        Vector3 respawnPosition = deadPlayer.transform.position;
    
        if (deadCarZone is not null)
        {
            Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
    
            respawnPosition = respawnPoint?.position ?? deadCarZone.transform.position;
        }
    
        deadPlayer.transform.position = respawnPosition;
        deadPlayer.SetActive(true);
    
        //Por qué coge el component si ya tiene la referencia arriba
        //deadCharacterController = deadPlayer.GetComponent<CharacterController>();
        deadCharacterController.enabled = true;
    
        deadPlayerHealth.ReviveToFullHealth();
        deadPlayerHealth.StartInvulnerability();
    
        //Lo mismo que con el charcontroller
        //deadPlayerMovement = deadPlayer.GetComponent<PlayerMovement>();
        deadPlayerMovement.currentState = PlayerState.Move;
        levelCamera.ClearOverrideTarget();
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
