using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour // JumpObject 클래스는 MonoBehaviour를 상속받아 Unity의 컴포넌트로 동작합니다.
{
    // JumpForce 변수는 점프의 힘을 정의합니다. 기본값은 100f입니다.
    public float JumpForce = 100f;

    private void OnCollisionEnter(Collision collision) // OnCollisionEnter 메서드는 다른 물체와 충돌할 때 호출됩니다.
    {
        // 충돌한 물체의 Rigidbody 컴포넌트를 가져옵니다.
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>(); 

        if (rb != null) // Rigidbody 컴포넌트가 존재하는지 확인합니다.
        {
            // Rigidbody에 위 방향으로 JumpForce만큼의 순간적인 힘을 가합니다.
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse); 
        }
    }
}
