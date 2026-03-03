using System;
using UnityEngine;

public class TrainLifeManager : MonoBehaviour
{
    [SerializeField] private int maxTrainLife;
    private float currentTrainLife;
    
    private ProgressManager progressManager;

    private void Start()
    {
        progressManager = GetComponent<ProgressManager>();
    }

    public void TakeDamage(float amount)
    {
        currentTrainLife -= amount;

        if (currentTrainLife <= 0)
        {
            TrainDead();
        }
    }

    private void TrainDead()
    {
        progressManager.GameOver();
    }
}
