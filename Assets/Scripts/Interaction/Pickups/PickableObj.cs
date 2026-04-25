using System;
using UnityEngine;

public class PickableObj : MonoBehaviour, IInteractable
{
    [SerializeField] private ResourceType _type;
    public ResourceType type => _type;
    
    private Transform parentTransform;

    private PickableState currentState = PickableState.Ground;

    [SerializeField] private float xThrowSpeed;
    [SerializeField] private float maxYThrowSpeed;
    private float yThrowSpeed;
    private Vector3 throwDirection;
    private float gravity = 0.2f;

    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        switch (currentState)
        {
            case PickableState.Ground:
                
                break;
            
            case PickableState.Picked:
                
                this.transform.position = parentTransform.position + new Vector3(0, 1f, 0);

                break;
            
            case PickableState.Throw:
                
                yThrowSpeed -= gravity * Time.deltaTime;
                
                Vector3 horizontalSpeed = throwDirection * (xThrowSpeed * Time.deltaTime);
                horizontalSpeed.y = yThrowSpeed;
                
                this.transform.position += horizontalSpeed;
                
                if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit,  0.2f, groundLayer))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    {
                        currentState = PickableState.Ground;
                    }
                }
                
                break;
        }
    }

    public void OnPick(Transform transform)
    {
        parentTransform = transform;
        currentState = PickableState.Picked;
    }

    public void Drop(Vector3 lookDir)
    {
        this.transform.position = parentTransform.position + lookDir - new Vector3(0, 1f, 0);
        currentState = PickableState.Ground;
    }

    public virtual void Throw(Vector3 throwDir, out bool dropObj)
    {
        dropObj = true;
        
        throwDirection = throwDir;
        yThrowSpeed = maxYThrowSpeed;
        currentState = PickableState.Throw;
    }
}
