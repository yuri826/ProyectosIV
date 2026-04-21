using UnityEngine;

public class CollapseRockSpawnPoint : MonoBehaviour
{
    private bool isOccupied = false;

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}
