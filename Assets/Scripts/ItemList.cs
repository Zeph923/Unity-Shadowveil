// ������������� �� ItemName ��� ItemCategory ��� ������������� ��� ItemEnums.cs
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public ItemName itemName; // ����� ������������
        public ItemCategory itemCategory; // ��������� ������������
    }

    [SerializeField]
    private List<Item> items; // ����� ������������

    private void Start()
    {
        // �������� ���� ��� ������������ ��� ��� ���������� ����
        foreach (var item in items)
        {
            Debug.Log($"Item: {item.itemName}, Category: {item.itemCategory}");
        }
    }
}
