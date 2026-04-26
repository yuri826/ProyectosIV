using UnityEngine;

[CreateAssetMenu(fileName = "SandstormEvent", menuName = "Scriptable Objects/Level Events/Sandstorm Event")]
public class SandstormEvent : LevelEvent
{
    [SerializeField] private float duration = 10f;

    public override void ExecuteEvent()
    {
        SandstormSystem.Instance.StartSandstorm(duration);
    }
}
