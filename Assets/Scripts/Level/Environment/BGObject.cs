using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BGObject : MonoBehaviour
{
    private Vector3 moveDirection = -Vector3.right;

    public float spawnXPos { get; set; }
    public float despawnXPos { get; set; }
    public float[] spawnZLimits { get; set; } = new float[2];
    public float introAmount { get; set; }

    public void Move(float moveSpeed)
    {
        if (introAmount < 0) goto MOVE;
        
        introAmount -= Time.deltaTime;
        if (introAmount > 0) return;
        
        MOVE:
        
        this.transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        
        if (this.transform.position.x < despawnXPos)
        {
            MoveToSpawn();
        }
    }

    public void MoveToSpawn()
    {
        float zPos = Random.Range(spawnZLimits[0], spawnZLimits[1]);
        Vector3 newPos = new Vector3(spawnXPos, this.transform.position.y, zPos);
        this.transform.position = newPos;
    }
}
