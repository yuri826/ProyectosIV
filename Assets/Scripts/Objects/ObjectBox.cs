using UnityEngine;

public class ObjectBox : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _objectToSpawn;
    public GameObject objectToSpawn {get => _objectToSpawn;}
}
