using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;  // �������� ��������������
    [SerializeField] private float interactRadius = 1.5f;  // ����� ��� ��� ������ (���� ������ ����� � �������)
    [SerializeField] private LayerMask interactableLayer;  // �� Layer ��� ������������ �� interactable �����������

    void Update()
    {
        // ���� ������� �� ������� "E" ��� �������������
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformInteraction();
        }

        // ����������� ��� ������ ��� �� �� �������� ��� Scene View
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red); // ������� ���� ray ��� ������ ������������ ��� �����������
    }

    private void PerformInteraction()
    {
        // �������������� �� SphereCast ��� �� ��������� �� �������� ����������� ���� ��� ��� ������
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);

        if (colliders.Length > 0)  // �� �������� ����������� ���� ������� ��������������
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))  // ��������� �� �� ����������� ����� interactable
                {
                    // ������� ��� ������������� ��� ������������
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

    // ��� ������ ������������ ��� �������� �������������� ���� Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
