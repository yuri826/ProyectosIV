using UnityEngine;

public class CollapseRockKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CollapseRock rock))
        {
            return;
        }

        rock.RemoveRock();
    }
}
