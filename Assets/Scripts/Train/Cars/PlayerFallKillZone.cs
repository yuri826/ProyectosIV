using UnityEngine;

public class PlayerFallKillZone : MonoBehaviour
{
    [SerializeField] private TrainCarZone sourceCarZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealthManager otherPlayerHealth))
        {
            otherPlayerHealth.ForceDie();
        }
    }
}
