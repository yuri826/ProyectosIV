using System;
using UnityEngine;

public class CartEnemyManager : MonoBehaviour
{
    public static CartEnemyManager Instance;

    [field:SerializeField] public CartPoint[] cartPoints { get; set; }

    private void Awake()
    {
        Instance = this;
    }
}
