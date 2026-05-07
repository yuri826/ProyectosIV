using UnityEngine;

[CreateAssetMenu(fileName = "ArrowVolleyEvent", menuName = "Scriptable Objects/Level Events/Arrow Volley Event")]
public class ArrowVolleyEvent : LevelEvent
{
    [Header("Volley")]
    [SerializeField] private int volleyCount = 3;
    [SerializeField] private float timeBetweenVolleys = 1.5f;

    [Header("Warning")]
    [SerializeField] private float warningTime = 3f;

    [Header("Damage")]
    [SerializeField] private float damage = 1f;

    public override void ExecuteEvent()
    {
        int randomDir = Random.Range(0, 2);
        ArrowVolleyDirection dir = (ArrowVolleyDirection)randomDir;
        
        foreach (var cart in TrainGameMode.instance.GetCartManager().GetAllCarts())
        {
            cart.SpawnArrowVolley(dir);
        }
    }
}
