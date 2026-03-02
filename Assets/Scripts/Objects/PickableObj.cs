using System;
using UnityEngine;

public class PickableObj : MonoBehaviour, IInteractable
{
    [SerializeField] private string _type;
    public string type { get; private set; }
    
    private Transform parentTransform;

    private string state = "ground";

    [SerializeField] private float xThrowSpeed;
    [SerializeField] private float maxYThrowSpeed;
    private float yThrowSpeed;
    private Vector3 throwDirection;
    private float gravity = 0.2f;

    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        type = _type;
    }

    private void Update()
    {
        switch (state)
        {
            case "ground":
                
                break;
            
            case "picked":
                
                this.transform.position = parentTransform.position + new Vector3(0, 1f, 0);

                break;
            
            case "throw":
                
                yThrowSpeed -= gravity * Time.deltaTime;
                
                Vector3 horizontalSpeed = throwDirection * (xThrowSpeed * Time.deltaTime);
                horizontalSpeed.y = yThrowSpeed;
                
                this.transform.position += horizontalSpeed;
                
                if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit,  0.2f, groundLayer))
                {
                    Debug.Log(Vector3.Dot(hit.normal, Vector3.up));
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    {
                        state = "ground";
                    }
                }
                
                break;
        }
    }

    public void OnPick(Transform transform)
    {
        parentTransform = transform;
        state = "picked";
    }

    public void Drop(Vector3 lookDir)
    {
        this.transform.position = parentTransform.position + lookDir - new Vector3(0, 1f, 0);
        state = "ground";
    }

    public virtual void Throw(Vector3 throwDir, out bool dropObj)
    {
        dropObj = true;
        
        throwDirection = throwDir;
        yThrowSpeed = maxYThrowSpeed;
        state = "throw";
    }

    public string GetType()
    {
        return type;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, -transform.up);
    }
}
