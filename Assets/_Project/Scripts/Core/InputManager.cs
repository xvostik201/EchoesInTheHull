using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Echoes.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get;  private set; }
    
        private PlayerActionMap _playerActionMap;
    
        public event Action OnToggleFlashlight;
        public event Action OnInteract;

        public event Action OnSlot1;
        public event Action OnSlot2;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _playerActionMap = new PlayerActionMap();
        }

        private void OnEnable()
        {
            _playerActionMap.Enable();
        
            _playerActionMap.Flashlight.Toggle.performed += HandleFlashlight;
            _playerActionMap.Player.Interact.performed += HandleInteract;
        
            _playerActionMap.Equipment.Slot1.performed += ctx => OnSlot1?.Invoke();
            _playerActionMap.Equipment.Slot2.performed += ctx => OnSlot2?.Invoke();
        }

        private void OnDisable()
        {
            _playerActionMap.Flashlight.Toggle.performed -= HandleFlashlight;
            _playerActionMap.Player.Interact.performed -= HandleInteract;
        
            _playerActionMap.Disable();
        }

        private void HandleFlashlight(InputAction.CallbackContext context)
        {
            OnToggleFlashlight?.Invoke();
        }

        public Vector2 GetMoveDirection()
        {
            return _playerActionMap.Player.Move.ReadValue<Vector2>();
        }

        public Vector2 GetLookDelta()
        {
            return _playerActionMap.Player.Look.ReadValue<Vector2>();
        }
        public void HandleInteract(InputAction.CallbackContext context)
        {
            OnInteract?.Invoke();
        }
    }
}

