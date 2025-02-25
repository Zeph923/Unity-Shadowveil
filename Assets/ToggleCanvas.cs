using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
    [SerializeField] private GameObject playerCanvas; // Το Canvas που θα ενεργοποιούμε/απενεργοποιούμε

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Αν πατηθεί το I
        {
            if (playerCanvas != null)
            {
                bool isActive = playerCanvas.activeSelf;
                playerCanvas.SetActive(!isActive); // Εναλλαγή κατάστασης
            }
        }
    }
}
