using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    public Transform dropPosition;

    public void DropItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }
}
