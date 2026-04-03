using System.Collections;
using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 5f;
    
    [Header("Camera")]
    [SerializeField] private LevelCamera levelCamera;
    [SerializeField] private float respawnCameraSmoothSpeed = 1f;

    public void HandleDeath(PlayerHealthManager deadPlayerHealth)
    {
        if (deadPlayerHealth == null)
        {
            return;
        }

        GameObject deadPlayer = deadPlayerHealth.gameObject;
        PlayerMovement deadPlayerMovement = deadPlayer.GetComponent<PlayerMovement>();

        TrainCarZone deadCarZone = FindCarZoneForPosition(deadPlayer.transform.position);
        
        if (levelCamera != null && deadCarZone != null)
        {
            Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();

            if (respawnPoint != null)
            {
                levelCamera.SetOverrideTarget(respawnPoint, respawnCameraSmoothSpeed);
            }
        }

        NotifyOutlawsInDeadCar(deadCarZone, deadPlayerMovement);

        StartCoroutine(RespawnRoutine(deadPlayerHealth, deadCarZone));
    }

    private IEnumerator RespawnRoutine(PlayerHealthManager deadPlayerHealth, TrainCarZone deadCarZone)
    {
        if (deadPlayerHealth == null)
        {
            yield break;
        }

        GameObject deadPlayer = deadPlayerHealth.gameObject;
        PlayerMovement deadPlayerMovement = deadPlayer.GetComponent<PlayerMovement>();
        CharacterController deadCharacterController = deadPlayer.GetComponent<CharacterController>();

        if (deadPlayerMovement != null)
        {
            deadPlayerMovement.currentState = PlayerState.Locked;
        }

        if (deadCharacterController != null)
        {
            deadCharacterController.enabled = false;
        }

        deadPlayer.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        if (deadPlayer == null)
        {
            yield break;
        }

        Vector3 respawnPosition = deadPlayer.transform.position;

        if (deadCarZone != null)
        {
            Transform respawnPoint = deadCarZone.GetPlayerRespawnPoint();

            if (respawnPoint != null)
            {
                respawnPosition = respawnPoint.position;
            }
            else
            {
                respawnPosition = deadCarZone.transform.position;
            }
        }

        deadPlayer.transform.position = respawnPosition;
        deadPlayer.SetActive(true);

        deadCharacterController = deadPlayer.GetComponent<CharacterController>();
        if (deadCharacterController != null)
        {
            deadCharacterController.enabled = true;
        }

        deadPlayerHealth.ReviveToFullHealth();
        deadPlayerHealth.StartInvulnerability();

        deadPlayerMovement = deadPlayer.GetComponent<PlayerMovement>();
        if (deadPlayerMovement != null)
        {
            deadPlayerMovement.currentState = PlayerState.Move;
        }

        if (levelCamera != null)
        {
            levelCamera.ClearOverrideTarget();
        }
    }

    private TrainCarZone FindCarZoneForPosition(Vector3 worldPosition)
    {
        TrainCarZone[] trainCarZones = FindObjectsByType<TrainCarZone>(FindObjectsSortMode.None);

        for (int i = 0; i < trainCarZones.Length; i++)
        {
            if (trainCarZones[i] == null)
            {
                continue;
            }

            if (trainCarZones[i].ContainsPoint(worldPosition))
            {
                return trainCarZones[i];
            }
        }

        return null;
    }

    private void NotifyOutlawsInDeadCar(TrainCarZone deadCarZone, PlayerMovement deadPlayerMovement)
    {
        if (deadCarZone == null || deadPlayerMovement == null)
        {
            return;
        }

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

            outlaws[i].OnPlayerFell(deadPlayerMovement);
        }
    }
}
