using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedManager : MonoBehaviour
{
    public static SpeedManager instance;
    
    [Header("Speed Vars")]
    [SerializeField] private int initSpeed;
    
    [SerializeField] private int minSpeed;
    [SerializeField] private int maxSpeed;
    
    [SerializeField] private int lowSpeedThreshold;
    [SerializeField] private int highSpeedThreshold;
    
    public string speedState { get; set; } = "middle";
    private bool isOnIntro = true;

    private int currentSpeed;
    [Tooltip("Tiempo que tarda en bajar 1km/h")][SerializeField] private float maxSpeedTimer = 0.5f;
    private float speedTimer;
    
    [Header("UI")]
    [SerializeField] private Image speedBar;
    [SerializeField] private TextMeshProUGUI speedText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentSpeed = 0;
        speedTimer = maxSpeedTimer;
        UpdateHUD();
    }

    private void Update()
    {
        if (isOnIntro)
        {
            speedTimer -= Time.deltaTime;

            if (speedTimer <= 0)
            {
                currentSpeed++;
                speedTimer = maxSpeedTimer * 0.1f;
                UpdateHUD();

                if (currentSpeed >= 100)
                {
                    speedTimer = maxSpeedTimer * 3;
                    isOnIntro = false;
                }
            }
        }
        else
        {
            speedTimer -= Time.deltaTime;
        
            //Baja la velocidad
            if (speedTimer <= 0)
            {
                speedTimer = maxSpeedTimer;
                currentSpeed--;
                currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
            
                UpdateHUD();
            }

            //Cambia el estado de la velocidad, para que los objetos necesarios puedan leer qué deben hacer
            //según la velocidad actual
            if (currentSpeed < lowSpeedThreshold) speedState = "low";
            else if (currentSpeed > highSpeedThreshold) speedState = "high";
            else speedState = "middle";
        }
    }


    public void AddSpeed(int speed)
    {
        currentSpeed += speed;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

        UpdateHUD();
    }
    
    private void UpdateHUD()
    {
        speedBar.fillAmount = (float)(currentSpeed - minSpeed) / (float)(maxSpeed - minSpeed);
        speedText.text = $"{currentSpeed} km/h";
    }
}
