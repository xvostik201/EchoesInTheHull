using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Echoes.Player;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private Transform _leftDoor;
    [SerializeField] private Transform _rightDoor;
    
    private Vector3 _leftDoorStartPosition;
    private Vector3 _rightDoorStartPosition;

    [Header("Animation Settings")]
    [SerializeField] private float _xOffset = 1f;
    [SerializeField] private float _animateTime = 1f;

    private void Start()
    {
        _leftDoorStartPosition = _leftDoor.position;
        _rightDoorStartPosition = _rightDoor.position;
    }

    private void AnimateDoor(bool isOpen = true)
    {
        _leftDoor.DOKill();
        _rightDoor.DOKill();
        
        float leftDoorEndPos = isOpen ? _leftDoorStartPosition.x - _xOffset : _leftDoorStartPosition.x;
        float rightDoorEndPos = isOpen ? _rightDoorStartPosition.x + _xOffset : _rightDoorStartPosition.x;
        
        Debug.Log($"left door x - {leftDoorEndPos},  right door x - {rightDoorEndPos}");
        
        _leftDoor.DOMoveX(leftDoorEndPos, _animateTime);
        _rightDoor.DOMoveX(rightDoorEndPos, _animateTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            AnimateDoor();
        }
    }private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            AnimateDoor(false);
        }
    }
}
