using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리 UI에 네모 한칸을 개별 클래스로 작성
public class ItemSlot : MonoBehaviour
{
    public ItemData item;   // 아이템 데이터

    public UIInventory inventory;
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;  // 수량표시 Text
    private Outline outline;             // 선택시 Outline 표시위한 컴포넌트

    public int index;                    // 몇 번째 Slot인지 index 할당
    public bool equipped;                // 장착여부
    public int quantity;                 // 수량데이터

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}