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
    }

    private void OnDisable()
    {
        _playerActionMap.Flashlight.Toggle.performed -= HandleFlashlight;
        _playerActionMap.Disable();
    }

    private void HandleFlashlight(InputAction.CallbackContext context)
    {
        OnToggleFlashlight?.Invoke();
    }
}
