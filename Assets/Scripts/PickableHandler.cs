using UnityEngine;
using TMPro; // For TextMesh Pro
using System.Collections;

public class PickableHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData Item;
    [SerializeField] private CanvasGroup messageCanvasGroup; // CanvasGroup for the message
    [SerializeField] private TextMeshProUGUI itemMessage; // TextMesh Pro text for the message
    [SerializeField] private Renderer itemRenderer; // Renderer of the item
    [SerializeField] private Collider itemCollider; // Collider of the item
    [SerializeField] private Transform itemCanvas; // Canvas that displays the item name
    private Transform playerCamera => Camera.main.transform; // Reference to the player's camera

    private void Start()
    {
        // Ensure the CanvasGroup is initially invisible
        if (messageCanvasGroup != null)
        {
            messageCanvasGroup.alpha = 0f;
            messageCanvasGroup.interactable = false;
            messageCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        // Ensure the itemCanvas always faces the player
        if (itemCanvas != null && playerCamera != null)
        {
            itemCanvas.LookAt(playerCamera);
            itemCanvas.Rotate(0, 180, 0); // Adjust rotation to face correctly
        }
    }

    public void Interact()
    {
        Debug.Log($"You found {Item.name}!", Item); // Display message in Console

        DisplayMessage($"You found {Item.displayName}!"); // Show message on screen
        MakeItemInvisible(); // Hide the item and disable interaction
    }

    private void DisplayMessage(string message)
    {
        if (messageCanvasGroup != null && itemMessage != null)
        {
            itemMessage.text = message; // Set text
            StopAllCoroutines(); // Stop any ongoing coroutines
            StartCoroutine(ShowAndHideMessage()); // Show and hide the message
        }
    }

    private IEnumerator ShowAndHideMessage()
    {
        // Show message
        messageCanvasGroup.alpha = 1f;
        messageCanvasGroup.interactable = true;
        messageCanvasGroup.blocksRaycasts = true;

        yield return new WaitForSeconds(2f); // Wait for 2 seconds

        // Hide message
        messageCanvasGroup.alpha = 0f;
        messageCanvasGroup.interactable = false;
        messageCanvasGroup.blocksRaycasts = false;
    }

    private void MakeItemInvisible()
    {
        // Hide the item
        if (itemRenderer != null)
            itemRenderer.enabled = false;

        // Disable Collider to prevent further interaction
        if (itemCollider != null)
            itemCollider.enabled = false;

        // Hide the name Canvas
        if (itemCanvas != null)
            itemCanvas.gameObject.SetActive(false);
    }
}