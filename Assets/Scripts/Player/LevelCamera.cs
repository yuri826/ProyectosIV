using System;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    private Vector3 lookOffset;
    private Quaternion lookRotOffset;

    private void Start()
    {
        lookOffset = transform.position;
        lookRotOffset = transform.rotation;
    }

    private void LateUpdate()
    {
        this.transform.position = lookAt.position + lookOffset;
    }
}
