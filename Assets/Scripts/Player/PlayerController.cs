using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

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
    
    private CharacterController _characterController;
    private CinemachineBasicMultiChannelPerlin _noise;
    private float _verticalVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _characterController = GetComponent<CharacterController>();
        _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
        
        _characterController.Move(finalVelocity * Time.deltaTime);
    }
    
    private void Look()
    {
        Vector2 lookDelta = InputManager.Instance.GetLookDelta();

        float mouseX = lookDelta.x * _mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
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
            Debug.Log($"Interact with {hit.collider.name}");
        }
    }
}
