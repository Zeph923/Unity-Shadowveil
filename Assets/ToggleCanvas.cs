using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
    [SerializeField] private GameObject playerCanvas; // �� Canvas ��� �� �������������/���������������

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // �� ������� �� I
        {
            if (playerCanvas != null)
            {
                bool isActive = playerCanvas.activeSelf;
                playerCanvas.SetActive(!isActive); // �������� ����������
            }
        }
    }
}
