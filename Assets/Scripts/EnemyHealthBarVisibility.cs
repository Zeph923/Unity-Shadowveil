using UnityEngine;
using TMPro;

public class EnemyHealthBarVisibility : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject healthBar; // � ����� ������ ��� ������
    [SerializeField] private Transform player;    // � �������
    [SerializeField] private TMP_Text enemyNameText;  // �� ������� �� �� ����� ��� ������ (TextMeshPro)
    [SerializeField] private float visibilityRange = 10f; // �������� �������������
    [SerializeField] private float heightOffset = 2f; // ���� ��� ������ ���� ��� ��� �����
    [SerializeField] private float nameTextOffset = 0.5f; // ���� ��� �������� ���� ��� �� ����� ������

    private Camera mainCamera;

    private void Start()
    {
        // ��������� ��� ����� ������
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // ����������� ��� ��������� ������ ������ ��� ������
        float distance = Vector3.Distance(transform.position, player.position);

        // ������������ � �������������� ��� ������ ������ ��� ��� ��������
        bool isVisible = distance <= visibilityRange;
        healthBar.SetActive(isVisible);
        if (enemyNameText != null)
        {
            enemyNameText.gameObject.SetActive(isVisible);
        }

        // ��������� ��� ����� ��� ��� ����������� ��� ������ ������ ��� ��� ��������
        if (isVisible)
        {
            Vector3 worldPos = transform.position + Vector3.up * heightOffset;
            healthBar.transform.position = worldPos;

            // ���������� ��� ������ ���� �� ������� ����� ��� ������
            if (mainCamera != null)
            {
                healthBar.transform.LookAt(mainCamera.transform);
                healthBar.transform.Rotate(0, 180, 0); // ���������� ��� ����� ���

                // ��������� ��� �� ������� ��������
                if (enemyNameText != null)
                {
                    Vector3 nameWorldPos = worldPos + Vector3.up * nameTextOffset;
                    enemyNameText.transform.position = nameWorldPos;
                    enemyNameText.transform.LookAt(mainCamera.transform);
                    enemyNameText.transform.Rotate(0, 180, 0); // ���������� ��� ����� ���
                }
            }
        }
    }
}
