using System;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField] private float cameraMoveQ;
    [SerializeField] private float cameraRotateQ;
    
    private Transform pointToMove;
    
    public void MoveToPos(Transform point)
    {
        pointToMove = point;
    }

    private void Update()
    {
        Debug.Log(this.transform.position);
        
        this.transform.position = Vector3.Lerp(this.transform.position, pointToMove.position, Time.deltaTime * cameraMoveQ);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, pointToMove.rotation, Time.deltaTime * cameraRotateQ);
    }
}
