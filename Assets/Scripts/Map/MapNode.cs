using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    [SerializeField] private Transform cursorPosition;
    [SerializeField] private Transform cameraPosition;

    public Transform CursorPos => cursorPosition;
    public Transform CameraPosition => cameraPosition;

    [Tooltip("SOLO PARA EL PRIMERO! / para debugear")][SerializeField] private bool activeInit;
    [SerializeField] private MapNode nextNode;
    
    //[SerializeField] private Button nodeButton;
    //[SerializeField] private Button playButton;
    public string state = "locked"; //Should be private, public for debugging

    [SerializeField] private GameObject nodeInfo;

    private void Start()
    {
        if (activeInit) state = "unlocked";
        //nodeButton.onClick.AddListener(NodeClicked);
    }

    private void NodeClicked()
    {
        switch (state)
        {
            case "locked": break;
            
            case "unlocked": break;
            
            case "completed": break;
        }
    }

    public void CloseInfo()
    {
        nodeInfo.SetActive(false);
    }

    public void OpenInfo()
    {
        nodeInfo.SetActive(true);
    }
}
