using UnityEngine;

public class PickableObj : MonoBehaviour, IInteractable
{
    private Transform parentTransform;

    private string state = "ground";

    private void Update()
    {
        switch (state)
        {
            case "ground":
                
                break;
            
            case "picked":
                
                this.transform.position = parentTransform.position + new Vector3(0, 1f, 0);

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
}
