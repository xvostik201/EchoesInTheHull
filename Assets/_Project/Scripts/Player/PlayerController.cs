using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Echoes.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Echoes.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Move Settings")] 
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _mouseSensitivity = 8f;
        
        [Header("Gravity")]
        [SerializeField] private float _gravity = -9.8f;
    
        [Header("Camera Settings")] 
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _shakeAmount = 1f;
    
        [Header("Interaction")]
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private float _rayDistance = 5f;
    
        [Header("Cursor")] 
        [SerializeField] private Image _cursor;
        [SerializeField] private Color _defaultCursorColor;
        [SerializeField] private Color _interactionCursorColor;

        [Header("Sound settings")]
        [SerializeField] private GameObject _sound;

        [SerializeField] private float _stepTime = 1f;
        private float _stepTimer;
        
        private CharacterController _characterController;
        private CinemachineBasicMultiChannelPerlin _noise;
        private CinemachinePOV _pov;
        private float _verticalVelocity;
        private float _ySpeed;
    
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    
            _ySpeed = _pov.m_VerticalAxis.m_MaxSpeed;
            Debug.Log(_ySpeed);
        }
    
        private void OnEnable()
        {
            InputManager.Instance.OnInteract += PerformInteraction;
        }
    
        private void OnDisable()
        {
            InputManager.Instance.OnInteract -= PerformInteraction;
        }
    
        void Update()
        {
            Move();
            Look();
            UpdateCursorUI();
        }
    
        private void Move()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0) _verticalVelocity = -2f;
    
            Vector2 input = InputManager.Instance.GetMoveDirection();
    
            Vector3 moveDirection = (transform.forward * input.y + transform.right * input.x).normalized;
    
            _verticalVelocity += _gravity * Time.deltaTime;
            Vector3 finalVelocity = moveDirection * _moveSpeed;
            finalVelocity.y = _verticalVelocity;

            if (moveDirection.magnitude > 0.1f)
            {
                _stepTimer += Time.deltaTime;
                if (_stepTimer >= _stepTime)
                {
                    GameObject sound = Instantiate(_sound, transform.position, Quaternion.identity);
                    _stepTimer = 0;
                }
            }
            
            _characterController.Move(finalVelocity * Time.deltaTime);
        }
        
        private void Look()
        {
            bool isInteracting = EquipmentManager.Instance.Tablet.IsInteracting;
            bool inExitZone = EquipmentManager.Instance.Tablet.IsMouseInExitZone();
    
            if (isInteracting)
            {
                if (!inExitZone)
                {
                    SetCinemachineInput(0); 
                    return;
                }
                else
                {
                    SetCinemachineInput(_ySpeed); 
                }
            }
            else
            {
                SetCinemachineInput(_ySpeed); 
                Vector2 lookDelta = InputManager.Instance.GetLookDelta();
                transform.Rotate(Vector3.up * lookDelta.x * _mouseSensitivity * Time.deltaTime);
            }
        }
    
        private void SetCinemachineInput(float speed)
        {
            if (_pov != null)
            {
                _pov.m_VerticalAxis.m_MaxSpeed = speed;
            }
        }
    
        private void UpdateCursorUI()
        {
            Ray ray = new Ray(_virtualCamera.transform.position, _virtualCamera.transform.forward);
            _cursor.color = Physics.Raycast(ray, _rayDistance, _interactionLayer) 
                ? _interactionCursorColor 
                : _defaultCursorColor;
        }
    
        private void PerformInteraction()
        {
            Ray ray = new Ray(_virtualCamera.transform.position, _virtualCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _interactionLayer))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                    Debug.Log($"Interact with {hit.collider.name}");
                }
            }
        }
    }
}
