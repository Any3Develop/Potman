#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Potman.ShowRoom
{
    public class ShowRoomCameraTarget : MonoBehaviour
    {
        private InputAction activate;
        private static ShowRoomCameraTarget current;
        private CinemachineCamera vurtialCamera;
        private CinemachineInputAxisController inputAxisController;
        private GraphicRaycaster uiRaycaster;
        private void Awake()
        {
            uiRaycaster = FindAnyObjectByType<GraphicRaycaster>();
            vurtialCamera = FindAnyObjectByType<CinemachineCamera>();
            inputAxisController = vurtialCamera.GetComponent<CinemachineInputAxisController>();
            ShowRoomWindow.Instance.CreateButton($"Camera {name}", name, SetAsCurrent);

            if (!current)
                SetAsCurrent();

            // Создаем InputAction с типом Button и биндом на ЛКМ
            activate = new InputAction("MouseClick", InputActionType.Button, "<Mouse>/leftButton");

            // Подписываемся на события нажатия и отпускания
            activate.started += ActivateOnperformed;
            activate.canceled += ActivateOnperformed;
            activate.Enable();
        }

        private void Start()
        {
            inputAxisController.enabled = false;
        }
        
        private bool IsPointerOverUI()
        {
            if (uiRaycaster == null) return false; // Проверяем, назначен ли Raycaster

            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue() // Берем текущую позицию мыши
            };

            var results = new List<RaycastResult>();
            uiRaycaster.Raycast(eventData, results);

            return results.Count > 0; // Если что-то найдено — курсор над UI
        }
        
        private void ActivateOnperformed(InputAction.CallbackContext obj)
        {
            if (obj.started && IsPointerOverUI())
                return;
            
            inputAxisController.enabled = obj.started;
        }

        private void SetAsCurrent()
        {
            current = this;
            vurtialCamera.Target.TrackingTarget = transform;
        }
    }
}
#endif