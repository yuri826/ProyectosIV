using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private MapNodeInfo nodeInfo;
    [SerializeField] private Image[] starImages;
    private int score;

    [SerializeField] private Sprite starOn;

    [SerializeField] private MapInfoTypewriter infoTypewriter;
    
    [SerializeField] private Transform cursorPosition;
    [SerializeField] private Transform cameraPosition;

    private StudioEventEmitter sfx;

    public Transform CursorPos => cursorPosition;
    public Transform CameraPosition => cameraPosition;

    //[Tooltip("SOLO PARA EL PRIMERO! / para debugear")][SerializeField] private bool activeInit;
    //[SerializeField] private MapNode nextNode;
    
    //[SerializeField] private Button nodeButton;
    //[SerializeField] private Button playButton;
    public MapNodeState currentState { get; private set; }


    private void Start()
    {
        //if (activeInit) currentState = MapNodeState.Unlocked;
        //nodeButton.onClick.AddListener(NodeClicked);

        currentState = GameInstance.instance.mapNodeStates[index];
        score = GameInstance.instance.levelScores[index];

        for (int i = 0; i < score; i++)
        {
            starImages[i].sprite = starOn;
        }
    }

    public string GetNodeScene()
    {
        if (currentState == MapNodeState.Locked) return "locked";

        return nodeInfo.SceneName;
    }

    public void ActivateNodeInfo()
    {
        sfx.Play();
        infoTypewriter.TypeText(nodeInfo.Title, nodeInfo.Info, 0.6f);
    }

    public void DeactivateNodeInfo()
    {
        infoTypewriter.StopTyping();
    }
}
