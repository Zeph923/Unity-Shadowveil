using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // Singleton instance
    public static InventorySystem Instance;

    [SerializeField] private GameObject inventoryUI; // Το UI του inventory
    private bool isInventoryOpen = false; // Κατάσταση του inventory

    private void Awake()
    {
        // Ελέγχει αν υπάρχει ήδη μια instance και αν όχι, την ρυθμίζει
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Απενεργοποιεί το inventory όταν ξεκινάει το παιχνίδι
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
    }

    private void Update()
    {
        // Έλεγχος για το πλήκτρο 'I' και αλλαγή κατάστασης του inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory(!isInventoryOpen);
        }
    }

    // Μέθοδος για να ανοίξεις/κλείσεις το inventory
    public void ToggleInventory(bool isOpen)
    {
        isInventoryOpen = isOpen;

        if (inventoryUI != null)
        {
            inventoryUI.SetActive(isOpen);
            Debug.Log("Inventory Toggled: " + isOpen);
        }
    }
}
