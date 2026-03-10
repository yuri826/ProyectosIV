using System;
using UnityEngine;

public class TrainLifeManager : MonoBehaviour
{
    public static TrainLifeManager instance;
    
    [SerializeField] private int maxTrainLife;
    private float currentTrainLife;
    
    private ProgressManager progressManager;

    private bool isDead = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        progressManager = GetComponent<ProgressManager>();
    }

    public void TakeDamage(float amount)
    {
        currentTrainLife -= amount;

        if (currentTrainLife <= 0 && !isDead)
        {
            TrainDead();
        }
    }

    private void TrainDead()
    {
        isDead = true;
        progressManager.GameOver();
    }
}
