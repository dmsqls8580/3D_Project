using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemDropper itemDropper; // �κ��丮�� ���� á�� �� �������� ����ϱ� ���� �ʿ�

    public ItemSlot[] slots;        // ���� �迭 (UI ���� ������Ʈ�� �����)
    public int curEquipIndex;       // ���� ���� ���� ���� �ε���

    public System.Action onInventoryChanged; // UI ������Ʈ �̺�Ʈ

    // ���� �ʱ�ȭ: UI�� �ڽ� ���� ������Ʈ�� �����Ͽ� ItemSlot �迭 ����
    public void InitSlots(Transform slotPanel, UIInventory inventory)
    {
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = inventory;
            slots[i].Clear();
        }
    }

    /// <summary>
    /// �������� �κ��丮�� �߰�
    /// - ���� �����ϸ� ���� ���Կ� �߰�
    /// - �ƴϸ� �� ���Կ� �߰�
    /// - ��� �����ϸ� �ʵ忡 ���
    /// </summary>
    public void AddItem(ItemData data)
    {
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                onInventoryChanged?.Invoke();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            onInventoryChanged?.Invoke();
            return;
        }
        // �κ��丮�� ���� �� ��� ������ ���
        itemDropper.DropItem(data);
    }

    public void EquipItem(int index)
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquipItem(curEquipIndex);
        }

        slots[index].equipped = true;
        curEquipIndex = index;
    }

    public void UnEquipItem(int index)
    {
        slots[index].equipped = false;
    }

    // ������ ���� (���� ���� �� 0�̸� ����)
    public void RemoveItem(int index)
    {
        slots[index].quantity--;
        if (slots[index].quantity <= 0)
        {
            slots[index].item = null;
        }
    }

    // ������ ���� ���� ������ ������ ���� �� ���� ������ ���� ��ȯ
    ItemSlot GetItemStack(ItemData data)
    {
        foreach (var slot in slots)
        {
            if (slot.item == data && slot.quantity < data.maxStackAmount)
                return slot;
        }
        return null;
    }

    // �� ������ ã�� ��ȯ
    ItemSlot GetEmptySlot()
    {
        foreach (var slot in slots)
        {
            if (slot.item == null)
                return slot;
        }
        return null;
    }
}
