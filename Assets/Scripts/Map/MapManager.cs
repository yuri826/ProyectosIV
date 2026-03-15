using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    [SerializeField] private LevelManager levelManager;
    
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
        //debería de ser currentLevel y luego animación hacia el next
        currentNode = nodeList[levelManager.NewLevel];
        
        for (var i = 0; i < levelManager.CurrentLevel; i++)
        {
            nodeList[i].SetState("Done");
        }
        
        nodeList[levelManager.NewLevel].SetState("Unlocked");
        
        for (var i = levelManager.NewLevel+1; i < nodeList.Length; i++)
        {
            nodeList[i].SetState("Locked");
        }
        
        currentNode.ActivateNodeInfo();
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
        currentNode.DeactivateNodeInfo();
        
        int oldCurrentNodeIndex = currentNodeIndex;
        
        currentNodeIndex += direction;
        currentNodeIndex = Mathf.Clamp(currentNodeIndex,0,levelManager.NewLevel);
        
        currentNode = nodeList[currentNodeIndex];
        
        if (currentNodeIndex != oldCurrentNodeIndex)
        {
            currentNode.ActivateNodeInfo();
            mapCursor.transform.position = currentNode.CursorPos.position;
            mapCamera.MoveToPos(currentNode.CameraPosition);
        }
    }
}
