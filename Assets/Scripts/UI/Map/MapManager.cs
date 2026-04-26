using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    private GameInstance gameInstance => GameInstance.instance;
    
    [Tooltip("RESPETAR ORDEN NUMERICO!")][SerializeField] private MapNode[] nodeList;
    [SerializeField] private GameObject mapCursor;
    [SerializeField] private MapCamera mapCamera;
    
    private PlayerInput playerInput;
    
    private int currentNodeIndex = 0;
    private MapNode currentNode;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        //Pone el nodo actual
        currentNodeIndex = gameInstance.lastLevel;
        currentNode = nodeList[currentNodeIndex];
        
        //Mueve la cámara, el cursor y activa la info
        currentNode.ActivateNodeInfo();
        mapCursor.transform.position = currentNode.CursorPos.position;
        mapCamera.MoveToPos(currentNode.CameraPosition);
    }

    //Input binding
    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("UI");
        playerInput.actions["Left"].started += MoveNodeLeft;
        playerInput.actions["Up"].started += MoveNodeLeft;
        playerInput.actions["Right"].started += MoveNodeRight;
        playerInput.actions["Down"].started += MoveNodeRight;
    }
    
    private void MoveNodeLeft(InputAction.CallbackContext obj){MoveNode(-1);}
    private void MoveNodeRight(InputAction.CallbackContext obj){MoveNode(1);}
    
    private void MoveNode(int direction)
    {
        int newIndex = currentNodeIndex + direction;
        
        // if ((newIndex >= nodeList.Length) 
        //     ||  (newIndex < 0) //Mira si está dentro de los nodos posibles
        //     || (nodeList[newIndex].currentState == MapNodeState.Locked)) return; //Mira si el siguiente está bloqueado

        if (newIndex >= nodeList.Length)
        {
            return;
        }
        
        if (newIndex < 0)
        {
            return;
        }

        if (nodeList[newIndex].currentState == MapNodeState.Locked)
        {
            print("locked");
        }
            
        //Desactiva la información del nodo
        currentNode.DeactivateNodeInfo();
        
        //Pone el siguiente nodo como el actual
        currentNodeIndex += direction;
        currentNode = nodeList[currentNodeIndex];
        
        //Activa la información, mueve el cursor y la cámara
        mapCursor.transform.position = currentNode.CursorPos.position;
        mapCamera.MoveToPos(currentNode.CameraPosition);
        currentNode.ActivateNodeInfo();
    }
}
