using UnityEngine;

public class MenuCanvasController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject defaultCanvas;

    private GameObject currentCanvas;
    private GameObject previousCanvas;

    private void Start()
    {
        if (defaultCanvas != null)
        {
            OpenCanvas(defaultCanvas);
        }
    }

    public void OpenCanvas(GameObject canvasToOpen)
    {
        if (canvasToOpen == null)
        {
            return;
        }

        if (currentCanvas != null && currentCanvas != canvasToOpen)
        {
            currentCanvas.SetActive(false);
            previousCanvas = currentCanvas;
        }

        canvasToOpen.SetActive(true);
        currentCanvas = canvasToOpen;
    }

    public void OpenCanvasFromCurrent(GameObject canvasToOpen)
    {
        if (canvasToOpen == null)
        {
            return;
        }

        if (currentCanvas != null)
        {
            previousCanvas = currentCanvas;
            currentCanvas.SetActive(false);
        }

        canvasToOpen.SetActive(true);
        currentCanvas = canvasToOpen;
    }

    public void BackToPreviousCanvas()
    {
        if (previousCanvas == null)
        {
            return;
        }

        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }

        previousCanvas.SetActive(true);
        currentCanvas = previousCanvas;
        previousCanvas = null;
    }

    public GameObject GetCurrentCanvas()
    {
        return currentCanvas;
    }
}
