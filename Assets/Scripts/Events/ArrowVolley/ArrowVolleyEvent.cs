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

    [Header("Direction")]
    [SerializeField] private bool useRandomDirections = true;
    [SerializeField] private ArrowVolleyDirection[] fixedDirections;

    public override void ExecuteEvent()
    {
        ArrowVolleySystem.Instance.StartArrowVolley(
            volleyCount,
            timeBetweenVolleys,
            warningTime,
            damage,
            useRandomDirections,
            fixedDirections
        );
    }
}
