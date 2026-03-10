using UnityEngine;

public class OutlawHealth : MonoBehaviour
{
    [Header("Vida del forajido")]
    [SerializeField] private float maxHealth;
    
    private float currentHealth;


    private void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        Debug.Log(gameObject.name + " ha recibido daño. Vida restante: " + currentHealth);
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    private void Die()
    {
        // Más adelante hay que meter:
        // - animación de muerte
        // - sonido
        // - avisar al spawner -1 forgido
        Destroy(gameObject);
    }
}
