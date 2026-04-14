using UnityEngine;

public class LevelEvent : ScriptableObject
{
    [field: SerializeField] public int execTime { get; set;}
    
    public virtual void ExecuteEvent()
    {
        
    }
}
