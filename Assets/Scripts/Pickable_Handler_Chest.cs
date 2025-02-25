using UnityEngine;
using TMPro;
using System.Collections;

public class Pickable_Handler_Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData Item;
    [SerializeField] private CanvasGroup messageCanvasGroup;
    [SerializeField] private TextMeshProUGUI itemMessage;
    [SerializeField] private Collider itemCollider;
    [SerializeField] private Transform itemCanvas;
    [SerializeField] private Animator chestAnimator; // Animator for the chest
    [SerializeField] private PlayerHealth playerHealth; // Reference to PlayerHealth script

    private bool isOpened = false; // Check if the chest has already been opened
    private Transform playerCamera => Camera.main.transform;

    private void Start()
    {
        if (messageCanvasGroup != null)
        {
            messageCanvasGroup.alpha = 0f;
            messageCanvasGroup.interactable = false;
            messageCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (itemCanvas != null && playerCamera != null)
        {
            itemCanvas.LookAt(playerCamera);
            itemCanvas.Rotate(0, 180, 0);
        }
    }

    public void Interact()
    {
        if (isOpened) return; // If it's already opened, stop
        isOpened = true;

        Debug.Log($"You found {Item.name}!", Item);
        DisplayMessage($"You found {Item.displayName}!");

        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Open"); // Triggers the opening animation
        }
        else
        {
            Debug.LogWarning("Animator not found for chest.");
        }

        // Call PickupPotion to add a healing potion to the player
        if (playerHealth != null)
        {
            playerHealth.PickupPotion();  // Add a healing potion to the player
        }

        DisableInteraction(); // Disables interaction after opening
    }

    private void DisableInteraction()
    {
        if (itemCollider != null)
            itemCollider.enabled = false; // Makes it non-interactable

        if (itemCanvas != null)
            itemCanvas.gameObject.SetActive(false); // Hides the name
    }

    private void DisplayMessage(string message)
    {
        if (messageCanvasGroup != null && itemMessage != null)
        {
            itemMessage.text = message;
            StopAllCoroutines();
            StartCoroutine(ShowAndHideMessage());
        }
    }

    private IEnumerator ShowAndHideMessage()
    {
        messageCanvasGroup.alpha = 1f;
        messageCanvasGroup.interactable = true;
        messageCanvasGroup.blocksRaycasts = true;

        yield return new WaitForSeconds(2f);

        messageCanvasGroup.alpha = 0f;
        messageCanvasGroup.interactable = false;
        messageCanvasGroup.blocksRaycasts = false;
    }
}
