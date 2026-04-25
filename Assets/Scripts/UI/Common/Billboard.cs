using UnityEngine;

public class SimpleBillboard : MonoBehaviour
{
    private Camera mainCamera => Camera.main;

    private void LateUpdate()
    {
        //Por qué setear la variable cada frame?
        // if (mainCamera == null)
        // {
        //     mainCamera = Camera.main;
        // }
        
        if (mainCamera is not null) transform.forward = mainCamera.transform.forward;
    }
}
