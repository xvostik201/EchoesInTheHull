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
    [SerializeField] private float _mouseSensitivity = 2f;
    
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

    void Update()
    {
        Look();
        Move();
        HandleInteraction();
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _mouseSensitivity);
    }

    private void Move()
    {
        if (_characterController.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 moveDirection = (transform.forward * v + transform.right * h).normalized;
        
        if (moveDirection.magnitude > 0 && _characterController.isGrounded)
        {
            _noise.m_AmplitudeGain = _shakeAmount;
        }
        else
        {
            _noise.m_AmplitudeGain = 0f;
        }

        _verticalVelocity += _gravity * Time.deltaTime;
        Vector3 finalVelocity = moveDirection * _moveSpeed;
        finalVelocity.y = _verticalVelocity;
        
        _characterController.Move(finalVelocity * Time.deltaTime);
    }

    private void HandleInteraction()
    {
        Ray ray = new Ray(_virtualCamera.transform.position, _virtualCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _rayDistance, _interactionLayer))
        {
            _cursor.color = _interactionCursorColor;
            
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.TryGetComponent(out Button button))
                {
                }
            }
        }
        else
        {
            _cursor.color = _defaultCursorColor;
        }
    }
}