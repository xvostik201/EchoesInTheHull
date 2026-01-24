using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get;  private set; }
    
    private PlayerActionMap _playerActionMap;
    
    public event Action OnToggleFlashlight;
    public event Action OnInteract;

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
        _playerActionMap.Player.Interact.performed += HandleFlashlight;
    }

    private void OnDisable()
    {
        _playerActionMap.Flashlight.Toggle.performed -= HandleFlashlight;
        _playerActionMap.Player.Interact.performed -= HandleFlashlight;
        
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
