using System;
using UnityEngine;

public class PushableObj : MonoBehaviour
{
    [SerializeField] private float friction;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " collided with " + other.name);
            Vector3 pushDir = transform.position - other.transform.position;
            pushDir.y = 0;
            this.transform.position += pushDir * friction;
        }
    }
}
