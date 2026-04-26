using UnityEngine;

[CreateAssetMenu(fileName = "CartEvent", menuName = "Scriptable Objects/Level Events/CartEvent")]
public class CartEvent : LevelEvent
{
    [SerializeField] private GameObject cartPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int cartCount = 1;

    public override void ExecuteEvent()
    {
        for (int i = 0; i < cartCount; i++)
        {
            Instantiate(cartPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
