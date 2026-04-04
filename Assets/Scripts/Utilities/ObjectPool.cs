using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    private ObjectPool<GameObject> objectPool;

    /*private void Awake()
    {
        objectPool = new ObjectPool<GameObject>(
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 50
        );
    }*/
}
