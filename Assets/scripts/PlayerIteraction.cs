using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // 1. Это обязательно для работы с текстом

public class PlayerIteraction : MonoBehaviour
{
    private const string TagUse = "Use";

    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private GameObject InteractionUI; // Весь объект (Canvas или Panel)
    [SerializeField] private TextMeshProUGUI interactionText; // 2. Ссылка конкретно на ТЕКСТ

    private InputAction m_interactAction;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask _defaultLayerMask;

    private void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");
        m_interactAction = playerMap.FindAction("Interact");
    }

    private void OnEnable() => InputActions.FindActionMap("Player").Enable();
    private void OnDisable() => InputActions.FindActionMap("Player").Disable();

    void Update()
    {
        bool hitSomething = false;
        string description = "";

        // Проверка луча
        if (Physics.Raycast(_playerCamera.position, _playerCamera.forward, out RaycastHit hit, interactDistance, _defaultLayerMask))
        {
            if (hit.transform.CompareTag(TagUse))
            {
                // Ищем скрипт двери
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    hitSomething = true;
                    // 3. ЗАБИРАЕМ ТЕКСТ ИЗ ДВЕРИ
                    description = interactable.GetDescription();

                    if (m_interactAction.WasPressedThisFrame())
                    {
                        interactable.Interact();
                    }
                }
            }
        }

        // Обновляем UI
        if (InteractionUI != null)
        {
            InteractionUI.SetActive(hitSomething);

            // 4. ПЕРЕЗАПИСЫВАЕМ "New Text" на данные из двери
            if (hitSomething && interactionText != null)
            {
                interactionText.text = description;
            }
        }
    }
}