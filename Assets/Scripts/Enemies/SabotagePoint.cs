using UnityEngine;

public class SabotagePoint : MonoBehaviour
{
    // Este script representa un punto del suelo que puede ser sabotado por un enemigo.
    // Cada punto puede estar en tres estados:
    // free -> nadie lo está usando
    // reserved -> un enemigo lo ha elegido como objetivo
    // broken -> el suelo está roto

    // Estoy tirando de strings para los estados porque he visto que Yuri lo estabas haciendo así, si no dime y lo cambio
    public string state = "free";
    
    [Header("Referencias visuales")]
    
    [SerializeField] private GameObject brokenVisual;
    [SerializeField] private Collider brokenCollider;
    
    [Header("Punto donde aparece la dinamita")]
    
    [SerializeField] private Transform dynamitePoint;

    public Transform GetDynamitePoint()
    {
        return dynamitePoint;
    }
    
    private void Start()
    {
        state = "free";
        brokenVisual.SetActive(false);
        brokenCollider.enabled = false;
    }
    
    // Este método lo usará el enemigo para saber si puede usar este punto para sabotear
    public bool CanBeTargeted()
    {
        return state == "free";
    }
    
    // Este método lo usa un enemigo cuando decide sabotear este punto
    // Lo reservamos para que otro enemigo no elija el mismo.
    public bool ReservePoint()
    {
        if (state != "free")
        {
            return false;
        }

        state = "reserved";
        return true;
    }
    
    // Si el enemigo cancela el sabotaje liberamos el punto.
    public void CancelReservation()
    {
        if (state == "reserved")
        {
            state = "free";
        }
    }
    
    // Cuando la dinamita explota se llama a este método
    public void BreakPoint()
    {
        state = "broken";
        brokenVisual.SetActive(true);
        brokenCollider.enabled = true;
    }
    
    // Cuando el jugador repara el suelo
    /// <summary>
    /// 
    /// </summary>
    public void RepairPoint()
    {
        state = "free";
        brokenVisual.SetActive(false);
        brokenCollider.enabled = false;
    }
}
