using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour // JumpObject Ŭ������ MonoBehaviour�� ��ӹ޾� Unity�� ������Ʈ�� �����մϴ�.
{
    // JumpForce ������ ������ ���� �����մϴ�. �⺻���� 100f�Դϴ�.
    public float JumpForce = 100f;

    private void OnCollisionEnter(Collision collision) // OnCollisionEnter �޼���� �ٸ� ��ü�� �浹�� �� ȣ��˴ϴ�.
    {
        // �浹�� ��ü�� Rigidbody ������Ʈ�� �����ɴϴ�.
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>(); 

        if (rb != null) // Rigidbody ������Ʈ�� �����ϴ��� Ȯ���մϴ�.
        {
            // Rigidbody�� �� �������� JumpForce��ŭ�� �������� ���� ���մϴ�.
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse); 
        }
    }
}
