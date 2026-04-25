using System.Collections;
using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    //DEPRECATED
    
    // [Header("Respawn")]
    // [SerializeField] private float respawnDelay = 5f;
    //
    // [Header("Camera")]
    // [SerializeField] private LevelCamera levelCamera;
    //
    // private Coroutine respawnRoutine;
    
    // public void HandleDeath(PlayerHealthManager deadPlayerHealth, PlayerMovement deadPlayerMovement, 
    //     CharacterController deadCharacterController, TrainCarZone forcedCarZone = null)
    // {
    //     GameObject deadPlayer = deadPlayerHealth.gameObject;
    //
    //     TrainCarZone deadCarZone = forcedCarZone ?? FindCarZoneForPosition(deadPlayer.transform.position);
    //
    //     if (deadCarZone is not null)
    //     {
    //         Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
    //
    //         levelCamera.SetOverrideTarget(respawnPoint, respawnCameraSmoothSpeed);
    //     }
    //
    //     NotifyOutlawsInDeadCar(deadCarZone, deadPlayerMovement);
    //
    //     //Si no puedes morir respawneando esto no hace falta
    //     if (respawnRoutine != null)
    //     {
    //         StopCoroutine(respawnRoutine);
    //     }
    //
    //     respawnRoutine = StartCoroutine(RespawnRoutine(deadPlayerHealth, deadPlayerMovement, 
    //         deadCharacterController, deadCarZone));
    // }
    
    // private IEnumerator RespawnRoutine(PlayerHealthManager deadPlayerHealth, PlayerMovement deadPlayerMovement, 
    //     CharacterController deadCharacterController, TrainCarZone deadCarZone)
    // {
    //     GameObject deadPlayer = deadPlayerHealth.gameObject;
    //
    //     deadPlayerMovement.currentState = PlayerState.Locked;
    //     deadCharacterController.enabled = false;
    //     deadPlayer.SetActive(false);
    //
    //     RespawnCountdownDisplay countdownDisplay = null;
    //
    //     if (deadCarZone is not null)
    //     {
    //         Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
    //
    //         countdownDisplay = respawnPoint.GetComponent<RespawnCountdownDisplay>();
    //     }
    //
    //     float remainingRespawnTime = respawnDelay;
    //
    //     while (remainingRespawnTime > 0f)
    //     {
    //         //Puesto como ? porque maybe puede petar por nulo
    //         countdownDisplay?.ShowTime(remainingRespawnTime);
    //
    //         yield return null;
    //         remainingRespawnTime -= Time.deltaTime;
    //     }
    //
    //     //Puesto como ? porque maybe puede petar por nulo
    //     countdownDisplay?.Hide();
    //
    //     Vector3 respawnPosition = deadPlayer.transform.position;
    //
    //     if (deadCarZone is not null)
    //     {
    //         Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();
    //
    //         respawnPosition = respawnPoint?.position ?? deadCarZone.transform.position;
    //     }
    //
    //     deadPlayer.transform.position = respawnPosition;
    //     deadPlayer.SetActive(true);
    //
    //     //Por qué coge el component si ya tiene la referencia arriba
    //     //deadCharacterController = deadPlayer.GetComponent<CharacterController>();
    //     deadCharacterController.enabled = true;
    //
    //     deadPlayerHealth.ReviveToFullHealth();
    //     deadPlayerHealth.StartInvulnerability();
    //
    //     //Lo mismo que con el charcontroller
    //     //deadPlayerMovement = deadPlayer.GetComponent<PlayerMovement>();
    //     deadPlayerMovement.currentState = PlayerState.Move;
    //     levelCamera.ClearOverrideTarget();
    //
    //     respawnRoutine = null;
    // }
    
    //Debería de estar en un manager de los traincarts
    // private TrainCarZone FindCarZoneForPosition(Vector3 worldPosition)
    // { 
    //     TrainCarZone[] trainCarZones = FindObjectsByType<TrainCarZone>(FindObjectsSortMode.None);
    //
    //     foreach (var cart in trainCarZones)
    //     {
    //         if (cart is null)
    //         {
    //             continue;
    //         }
    //
    //         if (cart.ContainsPoint(worldPosition))
    //         {
    //             return cart;
    //         }
    //     }
    //
    //     return null;
    // }
    
    // private void NotifyOutlawsInDeadCar(TrainCarZone deadCarZone, PlayerMovement deadPlayerMovement)
    // {
    //     if (deadCarZone == null)
    //     {
    //         return;
    //     }
    //
    //     OutlawSystem[] outlaws = FindObjectsByType<OutlawSystem>(FindObjectsSortMode.None);
    //
    //     foreach (var outlaw in outlaws)
    //     {
    //         if (outlaw == null)
    //         {
    //             continue;
    //         }
    //
    //         if (!deadCarZone.ContainsPoint(outlaw.transform.position))
    //         {
    //             continue;
    //         }
    //
    //         outlaw.OnPlayerFell(deadPlayerMovement);
    //     }
    // }
}
