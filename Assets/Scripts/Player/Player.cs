using System;
using UnityEngine;

// Player�� ���õ� ����� ��Ƶδ� ��.
// �̰��� ���� ��ɿ� ���� ����. (ex.CharacterManager.Instance.Player.controller)
public class Player : MonoBehaviour
{
    public PlayerController controller;

    private void Awake()
    {
        // �̱���Ŵ����� Player�� ������ �� �ְ� �����͸� �ѱ��.
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
    }
}