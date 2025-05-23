using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemDropper itemDropper; // 인벤토리가 가득 찼을 때 아이템을 드롭하기 위해 필요

    public ItemSlot[] slots;        // 슬롯 배열 (UI 슬롯 오브젝트와 연결됨)
    public int curEquipIndex;       // 현재 장착 중인 슬롯 인덱스

    public System.Action onInventoryChanged; // UI 업데이트 이벤트

    // 슬롯 초기화: UI의 자식 슬롯 오브젝트를 참조하여 ItemSlot 배열 구성
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
    /// 아이템을 인벤토리에 추가
    /// - 스택 가능하면 기존 슬롯에 추가
    /// - 아니면 빈 슬롯에 추가
    /// - 모두 실패하면 필드에 드롭
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
        // 인벤토리가 가득 찬 경우 아이템 드롭
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

    // 아이템 제거 (수량 감소 → 0이면 제거)
    public void RemoveItem(int index)
    {
        slots[index].quantity--;
        if (slots[index].quantity <= 0)
        {
            slots[index].item = null;
        }
    }

    // 기존에 보유 중인 동일한 아이템 슬롯 중 스택 가능한 슬롯 반환
    ItemSlot GetItemStack(ItemData data)
    {
        foreach (var slot in slots)
        {
            if (slot.item == data && slot.quantity < data.maxStackAmount)
                return slot;
        }
        return null;
    }

    // 빈 슬롯을 찾아 반환
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
