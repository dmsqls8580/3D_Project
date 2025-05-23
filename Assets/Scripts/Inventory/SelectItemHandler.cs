using TMPro;
using UnityEngine;

public class SelectedItemHandler : MonoBehaviour
{
    // UI 텍스트 컴포넌트들
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statNameText;
    public TextMeshProUGUI statValueText;

    // 버튼 오브젝트들
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    // 선택된 아이템 정보
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public InventoryManager inventoryManager; // 인벤토리 매니저 참조 (외부 연결 필요)

    // 아이템 선택 시 호출되며, 상세 정보와 버튼 상태를 UI에 반영.
    public void SelectItem(ItemSlot slot, int index)
    {
        selectedItem = slot;
        selectedItemIndex = index;

        // 텍스트 UI 설정
        nameText.text = slot.item.itemName;
        descriptionText.text = slot.item.description;

        statNameText.text = string.Empty;
        statValueText.text = string.Empty;

        // 소비 효과 텍스트로 나열
        foreach (var effect in slot.item.consumables)
        {
            statNameText.text += effect.type + "\n";
            statValueText.text += effect.value + "\n";
        }

        // 버튼 활성화 조건 설정
        useButton.SetActive(slot.item.type == ItemType.Consumable);
        equipButton.SetActive(slot.item.type == ItemType.Equipable && !slot.equipped);
        unEquipButton.SetActive(slot.item.type == ItemType.Equipable && slot.equipped);
        dropButton.SetActive(true);
    }

    // 선택 상태를 초기화하고 UI를 비웁니다.
    public void Clear()
    {
        selectedItem = null;
        nameText.text = descriptionText.text = statNameText.text = statValueText.text = string.Empty;
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // 현재 선택된 아이템 슬롯 반환
    public ItemSlot GetSelectedItemSlot() => selectedItem;

    // 현재 선택된 아이템 슬롯 인덱스 반환
    public int GetSelectedItemIndex() => selectedItemIndex;
}
