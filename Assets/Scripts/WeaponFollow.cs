using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    public Transform handSocket; // ��������� �� �� HandSocket (�� GameObject ��� ���� ��� ������)
    public Vector3 weaponOffset; // � ���������� ��� ����� ��� �� ����

    private void Update()
    {
        // �� ���� ��������� �� ���� ��� ��� ���������� ��� ������ ��� ������
        transform.position = handSocket.position + weaponOffset;
        transform.rotation = handSocket.rotation; // ����������� ��� �� ���� ������������� �� �� ����
    }
}
