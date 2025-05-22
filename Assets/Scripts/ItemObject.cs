using UnityEngine;

// ���ͷ��� ������ ��ü�� ����� �������̽�
public interface IInteractable
{
    public string GetInteractPrompt();  // UI�� ǥ���� ����
    public void OnInteract();           // ���ͷ��� ȣ��
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.itemName}\n{data.description}";
        return str;
    }

    public void OnInteract()
    {
        //Player ��ũ��Ʈ ���� ����
        //Player ��ũ��Ʈ�� ��ȣ�ۿ� ������ data �ѱ��.
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}