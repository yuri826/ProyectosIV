using UnityEngine;

public class PlayerFallKillZone : MonoBehaviour
{
    [SerializeField] private TrainCarZone sourceCarZone;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.ForceDie(sourceCarZone);
    }
}
