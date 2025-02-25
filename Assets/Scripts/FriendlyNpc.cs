using UnityEngine;
using TMPro; // For using TextMeshPro
using System.Collections; // For Coroutines

public class FriendlyNpc : MonoBehaviour, IInteractable // Implements the IInteractable interface
{
    [SerializeField] private string[] dialogues;  // NPC dialogues
    [SerializeField] private CanvasGroup dialogueCanvasGroup; // CanvasGroup for visibility control
    [SerializeField] private TextMeshProUGUI dialogueText; // TextMeshPro element for dialogue
    [SerializeField] private Transform canvasTransform; // Reference to the Canvas Transform
    private Transform camTransform; // Reference to the main camera transform

    private int currentDialogueIndex = 0;
    private bool firstDialogueShown = false;  // Flag for the first dialogue

    void Start()
    {
        camTransform = Camera.main.transform; // Get the main camera transform
    }

    void LateUpdate()
    {
        // Make the canvas always face the camera
        if (canvasTransform != null && camTransform != null)
        {
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - camTransform.position);
        }
    }

    public void Interact()
    {
        if (dialogues.Length == 0) return;  // If there are no dialogues, do nothing

        if (!firstDialogueShown)
        {
            // Display the first dialogue only once
            DisplayDialogue(dialogues[0]);
            firstDialogueShown = true;
            currentDialogueIndex = 1;  // Move to the next dialogue
        }
        else
        {
            // Display the remaining dialogues in a loop
            DisplayDialogue(dialogues[currentDialogueIndex]);

            // Update the index for the next dialogues
            currentDialogueIndex++;

            // Reset to the second dialogue when the cycle completes
            if (currentDialogueIndex >= dialogues.Length)
            {
                currentDialogueIndex = 1;
            }
        }
    }

    private void DisplayDialogue(string dialogue)
    {
        StopAllCoroutines(); // Stop any previous coroutine to properly handle dialogue display
        StartCoroutine(ShowDialogueCoroutine(dialogue));
    }

    private IEnumerator ShowDialogueCoroutine(string dialogue)
    {
        dialogueText.text = dialogue; // Set dialogue text
        dialogueCanvasGroup.alpha = 1f; // Show dialogue

        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        dialogueCanvasGroup.alpha = 0f; // Hide dialogue
    }
}
