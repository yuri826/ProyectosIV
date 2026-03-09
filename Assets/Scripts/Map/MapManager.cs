using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    [Tooltip("RESPETAR ORDEN NUMERICO!")][SerializeField] private MapNode[] nodeList;
    [SerializeField] private GameObject mapCursor;
    [SerializeField] private MapCamera mapCamera;
    
    private PlayerInput playerInput;
    
    private int currentNodeIndex = 0;
    private MapNode currentNode;

    private void Awake()
    {
        instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        currentNode = nodeList[0];
        mapCursor.transform.position = currentNode.CursorPos.position;
        mapCamera.MoveToPos(currentNode.CameraPosition);
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("UI");
        playerInput.actions["Left"].started += MoveNodeLeft;
        playerInput.actions["Right"].started += MoveNodeRight;
    }
    
    private void MoveNodeLeft(InputAction.CallbackContext obj){ MoveNode(-1);}
    private void MoveNodeRight(InputAction.CallbackContext obj){ MoveNode(1);}

    private void MoveNode(int direction)
    {
        currentNodeIndex += direction;
        currentNodeIndex = Mathf.Clamp(currentNodeIndex,0,nodeList.Length-1);
        
        currentNode = nodeList[currentNodeIndex];
        mapCursor.transform.position = currentNode.CursorPos.position;
        mapCamera.MoveToPos(currentNode.CameraPosition);
    }
}
