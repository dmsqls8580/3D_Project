using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable,
    Misc
}
public enum ConsumableType
{
    Hunger,
    Health
}

public enum EquipableTyep
{
    Weapon,
    Armor
}
public enum EffectType
{
    SpeedBoost,
    // 다른 효과 유형 추가 가능
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;
    public int value;
    public float effectDuration;
    public EffectType effectType;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}