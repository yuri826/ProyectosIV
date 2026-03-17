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
        currentTrainLife = maxTrainLife;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }
        
        currentTrainLife -= amount;
        currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);

        if (currentTrainLife <= 0 && !isDead)
        {
            TrainDead();
        }
    }
    public void RepairTrain(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentTrainLife += amount;
        currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);
    }

    public float GetCurrentTrainLife()
    {
        return currentTrainLife;
    }

    public float GetMaxTrainLife()
    {
        return maxTrainLife;
    }

    private void TrainDead()
    {
        if (isDead)
        {
            return;
        }
        
        PlayerSystem.instance.DeactivatePlayers(PlayerState.GameOver);
        isDead = true;
        progressManager.GameOver();
    }
}
