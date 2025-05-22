using TMPro;
using UnityEngine;

public class SelectedItemHandler : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statNameText;
    public TextMeshProUGUI statValueText;

    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public InventoryManager inventoryManager; // 연결 필요

    public void SelectItem(ItemSlot slot, int index)
    {
        selectedItem = slot;
        selectedItemIndex = index;

        nameText.text = slot.item.itemName;
        descriptionText.text = slot.item.description;

        statNameText.text = string.Empty;
        statValueText.text = string.Empty;

        foreach (var effect in slot.item.consumables)
        {
            statNameText.text += effect.type + "\n";
            statValueText.text += effect.value + "\n";
        }

        useButton.SetActive(slot.item.type == ItemType.Consumable);
        equipButton.SetActive(slot.item.type == ItemType.Equipable && !slot.equipped);
        unEquipButton.SetActive(slot.item.type == ItemType.Equipable && slot.equipped);
        dropButton.SetActive(true);
    }

    public void Clear()
    {
        selectedItem = null;
        nameText.text = descriptionText.text = statNameText.text = statValueText.text = string.Empty;
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public ItemSlot GetSelectedItemSlot() => selectedItem;
    public int GetSelectedItemIndex() => selectedItemIndex;
}
