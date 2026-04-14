using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private GameObject InteractionUI;

    private InputAction m_moveAction;
    private InputAction m_jumpAction; // опционально для прыжка

    private Vector2 m_moveAmt;

    [SerializeField] private CinemachineCamera m_camera;
    [SerializeField] private CharacterController m_characterController;

    public float WalkSpeed = 1.5f;
    public float JumpHeight = 1.0f;
    public float Gravity = -9.81f;

    private Vector3 m_velocity; // вертикальная скорость

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");
        m_moveAction = playerMap.FindAction("Move");
        m_jumpAction = playerMap.FindAction("Jump"); // если есть
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();

        // Прыжок
        if (m_jumpAction != null && m_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            m_velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        // Гравитация
        ApplyGravity();
    }

    private void FixedUpdate()
    {
        Walking();
    }

    private void Walking()
    {
        // Горизонтальное движение
        Vector3 moveDirection = (GetForward() * m_moveAmt.y + GetRight() * m_moveAmt.x).normalized;
        Vector3 horizontalMove = moveDirection * WalkSpeed * Time.deltaTime;
        // Вертикальное движение
        Vector3 verticalMove = new Vector3(0, m_velocity.y, 0) * Time.deltaTime;

        m_characterController.Move(horizontalMove + verticalMove);
    }

    private void ApplyGravity()
    {
        if (m_characterController.isGrounded && m_velocity.y < 0)
        {
            m_velocity.y = -2f; // прижим к земле
        }
        m_velocity.y += Gravity * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        return m_characterController.isGrounded;
    }

    private Vector3 GetForward()
    {
        Vector3 forward = m_camera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = m_camera.transform.right;
        right.y = 0;
        return right.normalized;
    }
}