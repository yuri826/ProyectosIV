using System;
using UnityEngine;

[Serializable]
public class LevelEvent : ScriptableObject
{
    [field: SerializeField] public Sprite eventIconSprite { get; private set; }
    
    public virtual void ExecuteEvent()
    {
        
    }
}
