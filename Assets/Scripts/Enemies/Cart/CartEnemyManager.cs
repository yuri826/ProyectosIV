using System;
using UnityEngine;

[Serializable]
public class CartEnemyManager : GamemodeSubsystem
{
    [field:SerializeField] public CartPoint[] cartPoints { get; set; }
    
    [SerializeField] private TrainCarZone[] trainCarZones;
    
    public TrainCarZone FindCarZoneForPosition(Vector3 worldPosition)
    { 
        foreach (var cart in trainCarZones)
        {
            if (cart is null)
            {
                continue;
            }
    
            if (cart.ContainsPoint(worldPosition))
            {
                return cart;
            }
        }
    
        return null;
    }
}
