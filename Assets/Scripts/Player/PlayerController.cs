using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed; // 이동 속도
    private Vector2 curMovementInput; // 현재 키보드 입력 (WASD)
    public float jumpPower; // 점프 힘
    public LayerMask groundLayerMask; // 바닥 판정을 위한 레이어 설정

    [Header("Look")]
    public Transform cameraContainer; // 카메라가 붙어 있는 트랜스폼
    public float minXLook; // 카메라 아래로 볼 수 있는 최대 각도
    public float maxXLook; // 카메라 위로 볼 수 있는 최대 각도
    private float camCurXRot; // 현재 카메라 X축 회전값
    public float lookSensitivity; // 마우스 민감도

    private Vector2 mouseDelta;  // 마우스 이동 값

    [HideInInspector]
    public bool canLook = true; // 카메라 회전 가능 여부 (인벤토리 열렸을 때 false로)

    public Action inventory; // 인벤토리 UI 열기 콜백 연결용

    private Rigidbody rigidbody; // 플레이어 리지드바디 캐시

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>(); // Rigidbody 캐싱
    }

    void Start()
    {
        // 커서 잠금 (게임 시작 시 마우스 숨기기)
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();  // 물리 이동 처리
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook(); // 마우스 이동에 따른 시야 회전 처리
        }
    }

    // 입력 시스템: 마우스 이동 처리
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    // 입력 시스템: 키보드 이동 처리 (WASD)
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

    // 입력 시스템: 점프 처리
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            // 위 방향으로 순간적인 힘을 가해 점프
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    // 실제 이동 처리
    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y; // y값은 유지 (중력)

        rigidbody.velocity = dir; // 속도 직접 설정
    }

    // 카메라 회전 처리 (상하 + 좌우)
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    // 땅에 닿아 있는지 판단 (Raycast 4개 사용)
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
                return true; // 하나라도 땅에 닿으면 true
            }
        }

        return false;
    }

    // 인벤토리 버튼 눌렀을 때 처리
    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke(); // 인벤토리 열기 이벤트 실행
            ToggleCursor(); // 마우스 커서 보이기/숨기기
        }
    }

    // 마우스 커서 잠금 상태 전환
    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle; // 카메라 회전 가능 여부도 함께 변경
    }


    private Coroutine speedBoostCoroutine;

    // 스피드 부스트 적용 (곱하기 배율, 지속 시간)
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine); // 기존 부스트 중단
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    // 일정 시간 동안 이동 속도 증가 코루틴
    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= multiplier;

        yield return new WaitForSeconds(duration); // 지속 시간 대기

        moveSpeed = originalSpeed; // 원래 속도로 복귀
        speedBoostCoroutine = null;
    }
}