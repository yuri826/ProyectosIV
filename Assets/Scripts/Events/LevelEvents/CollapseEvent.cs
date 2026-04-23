using UnityEngine;

[CreateAssetMenu(fileName = "CollapseEvent", menuName = "Scriptable Objects/Level Events/Collapse Event")]
public class CollapseEvent : LevelEvent
{
    [SerializeField] private float duration;
    [SerializeField] private int rockCount;

    public override void ExecuteEvent()
    {
        CollapseSystem.Instance.StartCollapse(duration, rockCount);
    }
}
