using UnityEngine;
using TMPro;
using System.Collections;

public class PickableHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData Item;
    [SerializeField] private CanvasGroup messageCanvasGroup;
    [SerializeField] private TextMeshProUGUI itemMessage;
    [SerializeField] private Renderer itemRenderer;
    [SerializeField] private Collider itemCollider;
    [SerializeField] private Transform itemCanvas;
    [SerializeField] private string specificItemName = "AladdinsScimitar"; //Το όνομα του συγκεκριμένου αντικειμένου

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
        Debug.Log($"You found {Item.name}!", Item);
        DisplayMessage($"You found {Item.displayName}!");
        MakeItemInvisible();
    }

    public bool IsSpecificItem()
    {
        return Item != null && Item.name == specificItemName; //Έλεγχος αν είναι το συγκεκριμένο αντικείμενο
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

    private void MakeItemInvisible()
    {
        if (itemRenderer != null)
            itemRenderer.enabled = false;

        if (itemCollider != null)
            itemCollider.enabled = false;

        if (itemCanvas != null)
            itemCanvas.gameObject.SetActive(false);
    }
}