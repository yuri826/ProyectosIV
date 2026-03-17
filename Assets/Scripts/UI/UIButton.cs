using System;
using UnityEngine;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private GameObject buttonAccent;
    [SerializeField] private CanvasMenu parentMenu;
    
    

    private void Start()
    {
        buttonAccent.SetActive(false);
    }

    public void OnHover()
    {
        
        buttonAccent.SetActive(true);
    }
    
    public void OnDeHover()
    {
        buttonAccent.SetActive(false);
    }

    public void OnSelect()
    {
        //parentMenu.enabled = false; ???
        onClick.Invoke();
    }
}
