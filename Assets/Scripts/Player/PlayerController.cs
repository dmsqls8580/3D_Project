using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;  // ���� �Է� ��
    public float jumpPower;
    public LayerMask groundLayerMask;  // ���̾� ����

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;  // �ּ� �þ߰�
    public float maxXLook;  // �ִ� �þ߰�
    private float camCurXRot;
    public float lookSensitivity; // ī�޶� �ΰ���

    private Vector2 mouseDelta;  // ���콺 ��ȭ��

    [HideInInspector]
    public bool canLook = true;

    public Action inventory;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // ���� ����
    private void FixedUpdate()
    {
        Move();
    }

    // ī�޶� ���� -> ��� ������ ������ ī�޶� ������
    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    // �Է°� ó��
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    // �Է°� ó��
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        // ���� �Է��� y ���� z ��(forward, �յ�)�� ���Ѵ�.
        // ���� �Է��� x ���� x ��(right, �¿�)�� ���Ѵ�.
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;  // ���⿡ �ӷ��� �����ش�.
        dir.y = rigidbody.velocity.y;  // y���� velocity(��ȭ��)�� y ���� �־��ش�.

        rigidbody.velocity = dir;  // ����� �ӵ��� velocity(��ȭ��)�� �־��ش�.
    }

    void CameraLook()
    {
        // ���콺 �������� ��ȭ��(mouseDelta)�� y(�� �Ʒ�)���� �ΰ����� ���Ѵ�.
        // ī�޶� �� �Ʒ��� ȸ���Ϸ��� rotation�� x ���� �־��ش�. -> �ǽ����� Ȯ��
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        // ���콺 �������� ��ȭ��(mouseDelta)�� x(�¿�)���� �ΰ����� ���Ѵ�.
        // ī�޶� �¿�� ȸ���Ϸ��� rotation�� y ���� �־��ش�. -> �ǽ����� Ȯ��
        // �¿� ȸ���� �÷��̾�(transform)�� ȸ�������ش�.
        // Why? ȸ����Ų ������ �������� �յ��¿� ���������ϴϱ�.
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    bool IsGrounded()
    {
        // 4���� Ray�� �����.
        // �÷��̾�(transform)�� �������� �յ��¿� 0.2�� ����߷���.
        // 0.01 ���� ��¦ ���� �ø���.
        // ���̶���Ʈ �κ��� �������� �� �� �κ��� ������ �м��غ�����.
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        // 4���� Ray �� groundLayerMask�� �ش��ϴ� ������Ʈ�� �浹�ߴ��� ��ȸ�Ѵ�.
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    //// ������ ȿ�� ���� �޼��� �߰�
    //public void ApplyEffect(ItemData item)
    //{
    //    if (item.effectType == ItemData.EffectType.SpeedBoost)
    //    {
    //        if (activeEffectCoroutine != null)
    //        {
    //            StopCoroutine(activeEffectCoroutine);
    //        }
    //        activeEffectCoroutine = StartCoroutine(SpeedBoost(item.effectDuration));
    //    }
    //}

    //private IEnumerator SpeedBoost(float duration)
    //{
    //    MoveSpeed = originalMoveSpeed * 2; // �ӵ��� �� ��� ����
    //    yield return new WaitForSeconds(duration); // ���� �ð� ���� ���
    //    MoveSpeed = originalMoveSpeed; // �ӵ��� ������� ����
    //    activeEffectCoroutine = null;
    //}

}