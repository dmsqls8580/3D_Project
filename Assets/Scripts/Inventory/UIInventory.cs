using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UIInventory : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public SelectedItemHandler selectedHandler;
    public ItemDropper itemDropper;

    public Transform slotPanel;
    public GameObject inventoryWindow;

    private void Start()
    {
        inventoryManager.InitSlots(slotPanel, this);

        inventoryManager.onInventoryChanged += UpdateUI;

        // Inventory UI 초기화 로직들
        inventoryWindow.SetActive(false);

        itemDropper.dropPosition = CharacterManager.Instance.Player.dropPosition;

        CharacterManager.Instance.Player.controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += () =>
        {
            var data = CharacterManager.Instance.Player.itemData;
            inventoryManager.AddItem(data);
            CharacterManager.Instance.Player.itemData = null;
        };
    }

    // Inventory 창 Open/Close 시 호출
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

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    // UI 정보 새로고침
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
    public void OnUseButton()
    {
        var selected = selectedHandler.GetSelectedItemSlot();
        var player = CharacterManager.Instance.Player;

        foreach (var effectData in selected.item.consumables)
        {
            var effect = ConsumableEffectFactory.GetEffect(effectData.type);
            effect?.Apply(player, effectData.value, effectData.duration);
        }

        inventoryManager.RemoveItem(selectedHandler.GetSelectedItemIndex());
        selectedHandler.Clear();
        UpdateUI();
    }

    public interface IConsumableEffect
    {
        void Apply(Player player, float value, float duration);
    }

    public class HealEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.condition.Heal(value);
        }
    }

    public class HungerEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.condition.Eat(value);
        }
    }

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

    public class SpeedBoostEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.controller.ApplySpeedBoost(value, duration);
        }
    }

    public void OnDropButton()
    {
        var selected = selectedHandler.GetSelectedItemSlot();
        itemDropper.DropItem(selected.item);
        
        inventoryManager.RemoveItem(selectedHandler.GetSelectedItemIndex());
        UpdateUI();
        selectedHandler.Clear();
    }

    public void OnEquipButton()
    {
        int index = selectedHandler.GetSelectedItemIndex();
        inventoryManager.EquipItem(index);
        UpdateUI();
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }

    public void OnUnEquipButton()
    {
        int index = selectedHandler.GetSelectedItemIndex();
        inventoryManager.UnEquipItem(index);
        UpdateUI();
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }

    public void SelectItem(int index)
    {
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }
}
