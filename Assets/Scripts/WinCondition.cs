using UnityEngine;
using TMPro;

public class WinCondition : MonoBehaviour
{
    [Header("UI Settings")]
    public Canvas winCanvas;        
    public TMP_Text winText;        
    private CanvasGroup canvasGroup; 

    private void Start()
    {
        if (winCanvas != null)
        {
            canvasGroup = winCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f; 
            }
            else
            {
                Debug.LogWarning("CanvasGroup not found on winCanvas. Please add one!");
            }
        }
    }

    public void TriggerWinCondition()
    {
        if (winCanvas != null)
        {
            winCanvas.gameObject.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; 
            }
        }

        if (winText != null)
        {
            winText.text = "THESPESIA";
        }
        else
        {
            Debug.LogWarning("Win Text is not assigned in the WinCondition script.");
        }
    }
}