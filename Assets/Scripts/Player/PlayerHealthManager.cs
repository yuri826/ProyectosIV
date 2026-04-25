using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerWeapon playerWeapon;

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
        if (isDead || isInvulnerable || !canTakeDamage) return;

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

    public void StartInvulnerability()
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
        
        //Variables locales
        TrainCarZone currentCarZone = TrainGameMode.instance.GetCartManager().FindCarZoneForPosition(transform.position);
        Transform respawnPoint = currentCarZone.GetPlayerRespawnPoint();

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
        
        playerMesh.SetActive(false);
        print("AWAWAWAW");
    
        Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
        RespawnCountdownDisplay countdownDisplay = respawnPoint.GetComponent<RespawnCountdownDisplay>();
        Vector3 respawnPosition = deadCarZone.transform.position;

         for (int i = respawnDelay; i > 0; i--)
         {
             countdownDisplay?.ShowTime(i);
             yield return new WaitForSeconds(1);
         }
    
        countdownDisplay?.Hide();

        deadPlayer.transform.position = respawnPosition;
        deadPlayer.SetActive(true);
        deadCharacterController.enabled = true;
        
        print("endHealthStuff");
    
        deadPlayerHealth.ReviveToFullHealth();
        deadPlayerHealth.StartInvulnerability();
    
        deadPlayerMovement.currentState = PlayerState.Move;
        
        playerMesh.SetActive(true);
        
        levelCamera.lookAt = playerMovement.gameObject.transform;
    }
}
