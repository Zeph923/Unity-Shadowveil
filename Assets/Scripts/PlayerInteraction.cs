using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;  // Απόσταση αλληλεπίδρασης
    [SerializeField] private float interactRadius = 1.5f;  // Ράδιο για την σφαίρα (πόσο μεγάλη είναι η περιοχή)
    [SerializeField] private LayerMask interactableLayer;  // Το Layer που περιλαμβάνει τα interactable αντικείμενα

    void Update()
    {
        // Όταν πατηθεί το πλήκτρο "E" για αλληλεπίδραση
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformInteraction();
        }

        // Σχεδιάζουμε την σφαίρα για να τη βλέπουμε στο Scene View
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red); // Κάνουμε έναν ray για οπτική αναπαράσταση της κατεύθυνσης
    }

    private void PerformInteraction()
    {
        // Χρησιμοποιούμε το SphereCast για να ελέγξουμε αν υπάρχουν αντικείμενα γύρω από τον παίκτη
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);

        if (colliders.Length > 0)  // Αν υπάρχουν αντικείμενα στην περιοχή αλληλεπίδρασης
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))  // Ελέγχουμε αν το αντικείμενο είναι interactable
                {
                    // Καλούμε την αλληλεπίδραση του αντικειμένου
                    interactable.Interact();
                    Debug.Log($"Interacted with {collider.gameObject.name}");
                }
            }
        }
        else
        {
            Debug.Log("No interactable object in range.");
        }
    }

    // Για οπτική αναπαράσταση της περιοχής αλληλεπίδρασης στον Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
