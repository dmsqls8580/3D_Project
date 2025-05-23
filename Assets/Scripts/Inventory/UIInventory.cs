using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UIInventory : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public SelectedItemHandler selectedHandler;
    public ItemDropper itemDropper;

    public Transform slotPanel;          // ���� UI�� ��ġ�� �θ� ��ü
    public GameObject inventoryWindow;   // �κ��丮 â ������Ʈ

    private void Start()
    {
        // ���� �ʱ�ȭ �� UI ���� �̺�Ʈ ���
        inventoryManager.InitSlots(slotPanel, this);
        inventoryManager.onInventoryChanged += UpdateUI;

        // ���� �� �κ��丮 ��Ȱ��ȭ
        inventoryWindow.SetActive(false);

        // ������ ��� ��ġ ����
        itemDropper.dropPosition = CharacterManager.Instance.Player.dropPosition;

        // �κ��丮 ��ư ���� �� Toggle ����ǵ��� ����
        CharacterManager.Instance.Player.controller.inventory += Toggle;

        // �÷��̾ �������� �������� �� �κ��丮�� �߰�
        CharacterManager.Instance.Player.addItem += () =>
        {
            var data = CharacterManager.Instance.Player.itemData;
            inventoryManager.AddItem(data);
            CharacterManager.Instance.Player.itemData = null;
        };
    }

    // �κ��丮 â ����/�ݱ�
    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    // �κ��丮 ���� �ִ��� Ȯ��
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    // �κ��丮 UI ���� ���� ����
    public void UpdateUI()
    {
        for (int i = 0; i < inventoryManager.slots.Length; i++)
        {
            if (inventoryManager.slots[i].item != null)
            {
                inventoryManager.slots[i].Set();
            }
            else
            {
                inventoryManager.slots[i].Clear();
            }
        }
    }

    // ������ ��� ��ư Ŭ�� �� ȣ��
    public void OnUseButton()
    {
        var selected = selectedHandler.GetSelectedItemSlot();
        var player = CharacterManager.Instance.Player;

        // �������� �Һ� ȿ���� ��ȸ�ϸ� ����
        foreach (var effectData in selected.item.consumables)
        {
            var effect = ConsumableEffectFactory.GetEffect(effectData.type);
            effect?.Apply(player, effectData.value, effectData.duration);
        }

        // ����� ������ ���� �� UI ����
        inventoryManager.RemoveItem(selectedHandler.GetSelectedItemIndex());
        selectedHandler.Clear();
        UpdateUI();
    }

    // �Һ� ������ ȿ�� �������̽� ����
    public interface IConsumableEffect
    {
        void Apply(Player player, float value, float duration);
    }

    // ü�� ȸ�� ȿ��
    public class HealEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.condition.Heal(value);
        }
    }

    // ��� ȸ�� ȿ��
    public class HungerEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.condition.Eat(value);
        }
    }

    // �Һ� ������ ȿ���� Ÿ�Ժ��� ��ȯ�ϴ� ���丮
    public static class ConsumableEffectFactory
    {
        public static IConsumableEffect GetEffect(ConsumableType type)
        {
            return type switch
            {
                ConsumableType.Health => new HealEffect(),
                ConsumableType.Hunger => new HungerEffect(),
                ConsumableType.Speed => new SpeedBoostEffect(),
                _ => null
            };
        }
    }

    // �̵� �ӵ� ���� ȿ��
    public class SpeedBoostEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.controller.ApplySpeedBoost(value, duration);
        }
    }

    // ������ ��� ��ư Ŭ�� �� ȣ��
    public void OnDropButton()
    {
        var selected = selectedHandler.GetSelectedItemSlot();
        itemDropper.DropItem(selected.item);
        
        inventoryManager.RemoveItem(selectedHandler.GetSelectedItemIndex());
        UpdateUI();
        selectedHandler.Clear();
    }

    // ������ ���� ��ư Ŭ�� �� ȣ��
    public void OnEquipButton()
    {
        int index = selectedHandler.GetSelectedItemIndex();
        inventoryManager.EquipItem(index);
        UpdateUI();
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }

    // ������ ���� ��ư Ŭ�� �� ȣ��
    public void OnUnEquipButton()
    {
        int index = selectedHandler.GetSelectedItemIndex();
        inventoryManager.UnEquipItem(index);
        UpdateUI();
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }

    // ������ ���� �� ���� �ε��� ����
    public void SelectItem(int index)
    {
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }
}
