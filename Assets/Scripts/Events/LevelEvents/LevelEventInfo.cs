using System;
using UnityEngine;

[Serializable]
public class LevelEventInfo
{
    [field :SerializeField] public int execTime { get; set; }
    [field: SerializeField] public LevelEvent levelEvent { get; set; }
}
