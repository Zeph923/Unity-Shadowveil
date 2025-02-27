// PlayerInteraction.cs
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private GameObject specificChildToActivate;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformInteraction();
        }

        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
    }

    private void PerformInteraction()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                    Debug.Log($"Interacted with {collider.gameObject.name}");

                    if (collider.TryGetComponent(out PickableHandler pickableHandler) && pickableHandler.IsSpecificItem())
                    {
                        if (specificChildToActivate != null)
                        {
                            specificChildToActivate.SetActive(true);
                            Debug.Log("Το συγκεκριμένο child ενεργοποιήθηκε!");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("No interactable object in range.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}