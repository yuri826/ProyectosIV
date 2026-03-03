using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    public List<MapNode> mapNodes { get; set; }= new List<MapNode>();

    private void Awake()
    {
        instance = this;
    }
}
