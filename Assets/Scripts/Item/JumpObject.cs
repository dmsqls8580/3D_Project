using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{
    // 점프의 힘을 정의. 기본값 100f
    public float JumpForce = 100f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>(); 

        if (rb != null) // Rigidbody 컴포넌트가 존재하는지 확인
        {
            // Rigidbody에 위 방향으로 JumpForce만큼의 순간적인 힘 AddForce
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse); 
        }
    }
}
