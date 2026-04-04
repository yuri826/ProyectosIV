using System;
using UnityEngine;

public class TrainLifeManager : MonoBehaviour
{
    //Deprecated
    
    // public static TrainLifeManager instance;
    //
    // [SerializeField] private int maxTrainLife;
    // public int MaxTrainLife => maxTrainLife;
    //
    // private float currentTrainLife;
    // public float CurrentTrainLife => currentTrainLife;
    //
    // private ProgressManager progressManager;
    //
    // private bool isDead = false;
    //
    // private void Awake()
    // {
    //     instance = this;
    // }
    //
    // private void Start()
    // {
    //     progressManager = GetComponent<ProgressManager>();
    //     currentTrainLife = maxTrainLife;
    // }
    //
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.UpArrow)) TakeDamage(10);
    //     if (Input.GetKeyDown(KeyCode.DownArrow)) TakeDamage(-10);
    // }
    //
    // public void TakeDamage(float amount)
    // {
    //     if (isDead)
    //     {
    //         return;
    //     }
    //     
    //     currentTrainLife -= amount;
    //     currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);
    //
    //     if (currentTrainLife <= 0 && !isDead)
    //     {
    //         TrainDead();
    //     }
    // }
    // public void RepairTrain(float amount)
    // {
    //     if (isDead)
    //     {
    //         return;
    //     }
    //
    //     currentTrainLife += amount;
    //     currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);
    // }
    //
    // public float GetCurrentTrainLife()
    // {
    //     return currentTrainLife;
    // }
    //
    // public float GetMaxTrainLife()
    // {
    //     return maxTrainLife;
    // }
    //
    // private void TrainDead()
    // {
    //     if (isDead)
    //     {
    //         return;
    //     }
    //     
    //     PlayerSystem.instance.DeactivatePlayers(PlayerState.GameOver);
    //     isDead = true;
    //     progressManager.GameOver();
    // }
}
