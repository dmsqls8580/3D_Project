using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �κ��丮 UI�� �׸� ��ĭ�� ���� Ŭ������ �ۼ�
public class ItemSlot : MonoBehaviour
{
    public ItemData item;   // ������ ������

    public UIInventory inventory;
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;  // ����ǥ�� Text
    private Outline outline;             // ���ý� Outline ǥ������ ������Ʈ

    public int index;                    // �� ��° Slot���� index �Ҵ�
    public bool equipped;                // ��������
    public int quantity;                 // ����������

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    // UI(���� �� ĭ) ������Ʈ�� ���� �Լ�
    // �����۵����Ϳ��� �ʿ��� ������ �� UI�� ǥ��
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

    // UI(���� �� ĭ)�� ������ ���� �� UI�� ����ִ� �Լ�
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    // ������ Ŭ������ �� �߻��ϴ� �Լ�.
    public void OnClickButton()
    {
        // �κ��丮�� SelectItem ȣ��, ���� ������ �ε����� ����.
        inventory.SelectItem(index);
    }
}