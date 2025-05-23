using TMPro;
using UnityEngine;

public class SelectedItemHandler : MonoBehaviour
{
    // UI �ؽ�Ʈ ������Ʈ��
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statNameText;
    public TextMeshProUGUI statValueText;

    // ��ư ������Ʈ��
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    // ���õ� ������ ����
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public InventoryManager inventoryManager; // �κ��丮 �Ŵ��� ���� (�ܺ� ���� �ʿ�)

    // ������ ���� �� ȣ��Ǹ�, �� ������ ��ư ���¸� UI�� �ݿ�.
    public void SelectItem(ItemSlot slot, int index)
    {
        selectedItem = slot;
        selectedItemIndex = index;

        // �ؽ�Ʈ UI ����
        nameText.text = slot.item.itemName;
        descriptionText.text = slot.item.description;

        statNameText.text = string.Empty;
        statValueText.text = string.Empty;

        // �Һ� ȿ�� �ؽ�Ʈ�� ����
        foreach (var effect in slot.item.consumables)
        {
            statNameText.text += effect.type + "\n";
            statValueText.text += effect.value + "\n";
        }

        // ��ư Ȱ��ȭ ���� ����
        useButton.SetActive(slot.item.type == ItemType.Consumable);
        equipButton.SetActive(slot.item.type == ItemType.Equipable && !slot.equipped);
        unEquipButton.SetActive(slot.item.type == ItemType.Equipable && slot.equipped);
        dropButton.SetActive(true);
    }

    // ���� ���¸� �ʱ�ȭ�ϰ� UI�� ���ϴ�.
    public void Clear()
    {
        selectedItem = null;
        nameText.text = descriptionText.text = statNameText.text = statValueText.text = string.Empty;
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // ���� ���õ� ������ ���� ��ȯ
    public ItemSlot GetSelectedItemSlot() => selectedItem;

    // ���� ���õ� ������ ���� �ε��� ��ȯ
    public int GetSelectedItemIndex() => selectedItemIndex;
}
