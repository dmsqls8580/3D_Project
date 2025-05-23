using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed; // �̵� �ӵ�
    private Vector2 curMovementInput; // ���� Ű���� �Է� (WASD)
    public float jumpPower; // ���� ��
    public LayerMask groundLayerMask; // �ٴ� ������ ���� ���̾� ����

    [Header("Look")]
    public Transform cameraContainer; // ī�޶� �پ� �ִ� Ʈ������
    public float minXLook; // ī�޶� �Ʒ��� �� �� �ִ� �ִ� ����
    public float maxXLook; // ī�޶� ���� �� �� �ִ� �ִ� ����
    private float camCurXRot; // ���� ī�޶� X�� ȸ����
    public float lookSensitivity; // ���콺 �ΰ���

    private Vector2 mouseDelta;  // ���콺 �̵� ��

    [HideInInspector]
    public bool canLook = true; // ī�޶� ȸ�� ���� ���� (�κ��丮 ������ �� false��)

    public Action inventory; // �κ��丮 UI ���� �ݹ� �����

    private Rigidbody rigidbody; // �÷��̾� ������ٵ� ĳ��

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>(); // Rigidbody ĳ��
    }

    void Start()
    {
        // Ŀ�� ��� (���� ���� �� ���콺 �����)
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();  // ���� �̵� ó��
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook(); // ���콺 �̵��� ���� �þ� ȸ�� ó��
        }
    }

    // �Է� �ý���: ���콺 �̵� ó��
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    // �Է� �ý���: Ű���� �̵� ó�� (WASD)
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

    // �Է� �ý���: ���� ó��
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            // �� �������� �������� ���� ���� ����
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    // ���� �̵� ó��
    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y; // y���� ���� (�߷�)

        rigidbody.velocity = dir; // �ӵ� ���� ����
    }

    // ī�޶� ȸ�� ó�� (���� + �¿�)
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    // ���� ��� �ִ��� �Ǵ� (Raycast 4�� ���)
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true; // �ϳ��� ���� ������ true
            }
        }

        return false;
    }

    // �κ��丮 ��ư ������ �� ó��
    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke(); // �κ��丮 ���� �̺�Ʈ ����
            ToggleCursor(); // ���콺 Ŀ�� ���̱�/�����
        }
    }

    // ���콺 Ŀ�� ��� ���� ��ȯ
    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle; // ī�޶� ȸ�� ���� ���ε� �Բ� ����
    }


    private Coroutine speedBoostCoroutine;

    // ���ǵ� �ν�Ʈ ���� (���ϱ� ����, ���� �ð�)
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine); // ���� �ν�Ʈ �ߴ�
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    // ���� �ð� ���� �̵� �ӵ� ���� �ڷ�ƾ
    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= multiplier;

        yield return new WaitForSeconds(duration); // ���� �ð� ���

        moveSpeed = originalSpeed; // ���� �ӵ��� ����
        speedBoostCoroutine = null;
    }
}