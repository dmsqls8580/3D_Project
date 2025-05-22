using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemDropper itemDropper; // 추가: 드랍 기능을 사용할 수 있도록

    public ItemSlot[] slots;
    public int curEquipIndex;

    public System.Action onInventoryChanged;

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

    public void RemoveItem(int index)
    {
        slots[index].quantity--;
        if (slots[index].quantity <= 0)
        {
            slots[index].item = null;
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        foreach (var slot in slots)
        {
            if (slot.item == data && slot.quantity < data.maxStackAmount)
                return slot;
        }
        return null;
    }

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
