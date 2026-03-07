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
    
    public string speedState { get; set; } = "normal";

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
        currentSpeed = initSpeed;
        speedTimer = maxSpeedTimer;
    }

    private void Update()
    {
        speedTimer -= Time.deltaTime;
        
        //Baja la velocidad
        if (speedTimer <= 0)
        {
            speedTimer = maxSpeedTimer;
            currentSpeed--;
            currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
            
            speedBar.fillAmount = (float)(currentSpeed - minSpeed) / (float)(maxSpeed - minSpeed);
            speedText.text = $"{currentSpeed} km/h";
        }

        //Cambia el estado de la velocidad, para que los objetos necesarios puedan leer qué deben hacer
        //según la velocidad actual
        if (currentSpeed < lowSpeedThreshold) speedState = "low";
        else if (currentSpeed > highSpeedThreshold) speedState = "high";
        else speedState = "middle";
    }

    public void AddSpeed(int speed)
    {
        currentSpeed += speed;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
        
        speedBar.fillAmount = (float)(currentSpeed - minSpeed) / (float)(maxSpeed - minSpeed);
        speedText.text = $"{currentSpeed} km/h";
    }
}
