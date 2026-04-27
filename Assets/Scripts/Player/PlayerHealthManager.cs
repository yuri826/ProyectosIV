using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [Header("References")]
    private PlayerMovement playerMovement;
    private PlayerWeapon playerWeapon;
    private PlayerAudioManager playerAudioManager;

    [Header("Respawn")]
    //[SerializeField] private PlayerRespawnManager playerRespawnManager;
    [SerializeField] private float respawnInvulnerabilityTime = 1.5f;

    [SerializeField] private GameObject playerMesh;

    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageCooldown = 1f;
    
    [Header("Respawn")]
    [SerializeField] private int respawnDelay = 5;
    
    [Header("Camera")]
    [SerializeField] private LevelCamera levelCamera;

    private float currentHealth;
    private bool isDead = false;
    private bool canTakeDamage = true;
    private Coroutine damageCooldownRoutine;

    private bool isInvulnerable = false;
    private Coroutine invulnerabilityRoutine;

    private void Awake()
    {
        playerAudioManager = GetComponent<PlayerAudioManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable || !canTakeDamage) return;
        
        playerAudioManager.PlaySfx(PlayerSFX.hurt);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            ForceDie();
            return;
        }

        if (damageCooldownRoutine != null) StopCoroutine(damageCooldownRoutine);
        damageCooldownRoutine = StartCoroutine(DamageCooldownRoutine());
    }

    private IEnumerator DamageCooldownRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
        damageCooldownRoutine = null;
    }

    private void StartInvulnerability()
    {
        if (invulnerabilityRoutine != null) StopCoroutine(invulnerabilityRoutine);

        invulnerabilityRoutine = StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(respawnInvulnerabilityTime);

        isInvulnerable = false;
    }

    public void ForceDie()
    {
        if (isDead) return;
        
        playerAudioManager.PlaySfx(PlayerSFX.die);
        
        //Variables locales
        TrainCarZone currentCarZone = TrainGameMode.instance.GetCartManager().FindCarZoneForPosition(transform.position);
        Transform respawnPoint = currentCarZone.playerRespawnPoint;

        //Setteo de variables
        isDead = true;
        canTakeDamage = false;

        //CDROutine
        if (damageCooldownRoutine is not null)
        {
            StopCoroutine(damageCooldownRoutine);
            damageCooldownRoutine = null;
        }

        //Player state
        playerMovement.ForceDropObj();
        playerWeapon.CancelReload();
        playerMovement.currentState = PlayerState.Locked;
        
        //Camara
        levelCamera.lookAt = respawnPoint;
    
        //Outlaws
        TrainGameMode.instance.GetTrainSpawnDirector().NotifyOutlawsInDeadCar(currentCarZone, playerMovement);
    
        //RespawnRoutine
        StartCoroutine(RespawnRoutine(this, playerMovement, 
            playerMovement.GetComponent<CharacterController>(), currentCarZone));
    }

    private void ReviveToFullHealth()
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
        //Bloquea y desaparece al jugador
        deadPlayerMovement.currentState = PlayerState.Locked;
        deadCharacterController.enabled = false;
        playerMesh.SetActive(false);
        
        //Variables locales respecto al respawn
        Transform respawnPoint = deadCarZone.playerRespawnPoint;
        RespawnCountdownDisplay countdownDisplay = respawnPoint.GetComponent<RespawnCountdownDisplay>();
        Vector3 respawnPosition = deadCarZone.transform.position;

        //Cuenta atrás del respawn
        for (int i = respawnDelay; i > 0; i--) 
        {
            countdownDisplay?.ShowTime(i); 
            yield return new WaitForSeconds(1);
        }
    
        countdownDisplay?.Hide();

        //Mueve al jugador
        var deadPlayer = deadPlayerHealth.gameObject;
        deadPlayer.transform.position = respawnPosition;

        //Reactiva los sistemas y la apariencia del jugador
        deadCharacterController.enabled = true;
        deadPlayerMovement.currentState = PlayerState.Move;
        playerMesh.SetActive(true);
        
        //Resetea la vida del jugador
        deadPlayerHealth.ReviveToFullHealth();
        deadPlayerHealth.StartInvulnerability();
    
        //La cámara mira al jugador de vuelta
        levelCamera.lookAt = playerMovement.gameObject.transform;
    }
}
