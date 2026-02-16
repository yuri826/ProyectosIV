using UnityEngine;

public class PickableObj : MonoBehaviour, IInteractable
{
    public Transform parentTransform { get; set; }

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

    public void OnPick()
    {
        state = "picked";
    }
}
