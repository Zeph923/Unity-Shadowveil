using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // Singleton instance
    public static InventorySystem Instance;

    [SerializeField] private GameObject inventoryUI; // �� UI ��� inventory
    private bool isInventoryOpen = false; // ��������� ��� inventory

    private void Awake()
    {
        // ������� �� ������� ��� ��� instance ��� �� ���, ��� ��������
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // ������������� �� inventory ���� �������� �� ��������
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
    }

    private void Update()
    {
        // ������� ��� �� ������� 'I' ��� ������ ���������� ��� inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory(!isInventoryOpen);
        }
    }

    // ������� ��� �� ��������/�������� �� inventory
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
