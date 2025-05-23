using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UIInventory : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public SelectedItemHandler selectedHandler;
    public ItemDropper itemDropper;

    public Transform slotPanel;          // 슬롯 UI를 배치할 부모 객체
    public GameObject inventoryWindow;   // 인벤토리 창 오브젝트

    private void Start()
    {
        // 슬롯 초기화 및 UI 갱신 이벤트 등록
        inventoryManager.InitSlots(slotPanel, this);
        inventoryManager.onInventoryChanged += UpdateUI;

        // 시작 시 인벤토리 비활성화
        inventoryWindow.SetActive(false);

        // 아이템 드롭 위치 설정
        itemDropper.dropPosition = CharacterManager.Instance.Player.dropPosition;

        // 인벤토리 버튼 누를 때 Toggle 실행되도록 연결
        CharacterManager.Instance.Player.controller.inventory += Toggle;

        // 플레이어가 아이템을 습득했을 때 인벤토리에 추가
        CharacterManager.Instance.Player.addItem += () =>
        {
            var data = CharacterManager.Instance.Player.itemData;
            inventoryManager.AddItem(data);
            CharacterManager.Instance.Player.itemData = null;
        };
    }

    // 인벤토리 창 열기/닫기
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

    // 인벤토리 열려 있는지 확인
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    // 인벤토리 UI 슬롯 정보 갱신
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

    // 아이템 사용 버튼 클릭 시 호출
    public void OnUseButton()
    {
        var selected = selectedHandler.GetSelectedItemSlot();
        var player = CharacterManager.Instance.Player;

        // 아이템의 소비 효과를 순회하며 적용
        foreach (var effectData in selected.item.consumables)
        {
            var effect = ConsumableEffectFactory.GetEffect(effectData.type);
            effect?.Apply(player, effectData.value, effectData.duration);
        }

        // 사용한 아이템 제거 및 UI 갱신
        inventoryManager.RemoveItem(selectedHandler.GetSelectedItemIndex());
        selectedHandler.Clear();
        UpdateUI();
    }

    // 소비 아이템 효과 인터페이스 정의
    public interface IConsumableEffect
    {
        void Apply(Player player, float value, float duration);
    }

    // 체력 회복 효과
    public class HealEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.condition.Heal(value);
        }
    }

    // 허기 회복 효과
    public class HungerEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.condition.Eat(value);
        }
    }

    // 소비 아이템 효과를 타입별로 반환하는 팩토리
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

    // 이동 속도 증가 효과
    public class SpeedBoostEffect : IConsumableEffect
    {
        public void Apply(Player player, float value, float duration)
        {
            player.controller.ApplySpeedBoost(value, duration);
        }
    }

    // 아이템 드롭 버튼 클릭 시 호출
    public void OnDropButton()
    {
        var selected = selectedHandler.GetSelectedItemSlot();
        itemDropper.DropItem(selected.item);
        
        inventoryManager.RemoveItem(selectedHandler.GetSelectedItemIndex());
        UpdateUI();
        selectedHandler.Clear();
    }

    // 아이템 장착 버튼 클릭 시 호출
    public void OnEquipButton()
    {
        int index = selectedHandler.GetSelectedItemIndex();
        inventoryManager.EquipItem(index);
        UpdateUI();
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }

    // 아이템 해제 버튼 클릭 시 호출
    public void OnUnEquipButton()
    {
        int index = selectedHandler.GetSelectedItemIndex();
        inventoryManager.UnEquipItem(index);
        UpdateUI();
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }

    // 아이템 선택 시 슬롯 인덱스 지정
    public void SelectItem(int index)
    {
        selectedHandler.SelectItem(inventoryManager.slots[index], index);
    }
}
