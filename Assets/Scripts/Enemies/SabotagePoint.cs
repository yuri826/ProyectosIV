using UnityEngine;

public class SabotagePoint : MonoBehaviour
{
    [Header("Sabotage State Point")]
    [SerializeField] private SabotagePointState currentState = SabotagePointState.Free;
    
    [Header("Visual References")]
    [SerializeField] private GameObject brokenVisual;
    [SerializeField] private Collider brokenCollider;
    
    [Header("Dinamite Appear Point")]
    [SerializeField] private Transform dynamitePoint;

    public Transform GetDynamitePoint()
    {
        return dynamitePoint;
    }
    
    private void Start()
    {
        currentState = SabotagePointState.Free;
        brokenVisual.SetActive(false);
        brokenCollider.enabled = false;
    }
    
    public bool CanBeTargeted()
    {
        return currentState == SabotagePointState.Free;
    }
    
    public bool IsBroken()
    {
        return currentState == SabotagePointState.Broken;
    }
    
    public bool ReservePoint()
    {
        if (currentState != SabotagePointState.Free)
        {
            return false;
        }

        currentState = SabotagePointState.Reserved;
        return true;
    }

    public void CancelReservation()
    {
        if (currentState == SabotagePointState.Reserved)
        {
            currentState = SabotagePointState.Free;
        }
    }
    
    public void BreakPoint()
    {
        currentState = SabotagePointState.Broken;
        brokenVisual.SetActive(true);
        brokenCollider.enabled = true;
    }

    public void RepairPoint()
    {
        currentState = SabotagePointState.Free;
        brokenVisual.SetActive(false);
        brokenCollider.enabled = false;
    }
}
