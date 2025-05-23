using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{
    // ������ ���� ����. �⺻�� 100f
    public float JumpForce = 100f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>(); 

        if (rb != null) // Rigidbody ������Ʈ�� �����ϴ��� Ȯ��
        {
            // Rigidbody�� �� �������� JumpForce��ŭ�� �������� �� AddForce
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse); 
        }
    }
}
