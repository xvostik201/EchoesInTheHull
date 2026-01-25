using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Echoes.Player;
using UnityEngine;

namespace Echoes.Interactions
{
    public class Door : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private Transform _leftDoor;
        [SerializeField] private Transform _rightDoor;

        [Header("Power")]
        [SerializeField] private bool _isPowerWorking = true;
    
        private Vector3 _leftDoorStartPosition;
        private Vector3 _rightDoorStartPosition;

        [Header("Animation Settings")]
        [SerializeField] private float _xOffset = 1f;
        [SerializeField] private float _animateTime = 1f;

        private void Start()
        {
            _leftDoorStartPosition = _leftDoor.localPosition;
            _rightDoorStartPosition = _rightDoor.localPosition;
        }

        private void AnimateDoor(bool isOpen = true)
        {
            _leftDoor.DOKill();
            _rightDoor.DOKill();
        
            float leftDoorEndPos = isOpen ? _leftDoorStartPosition.x - _xOffset : _leftDoorStartPosition.x;
            float rightDoorEndPos = isOpen ? _rightDoorStartPosition.x + _xOffset : _rightDoorStartPosition.x;
        
            Debug.Log($"left door x - {leftDoorEndPos},  right door x - {rightDoorEndPos}");
        
            _leftDoor.DOLocalMoveX(leftDoorEndPos, _animateTime);
            _rightDoor.DOLocalMoveX(rightDoorEndPos, _animateTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isPowerWorking) return;
            
            if (other.TryGetComponent(out PlayerController player))
            {
                AnimateDoor();
            }
        }private void OnTriggerExit(Collider other)
        {
            if (!_isPowerWorking) return;
            
            if (other.TryGetComponent(out PlayerController player))
            {
                AnimateDoor(false);
            }
        }

        public void Restart()
        {
            _isPowerWorking = true;
            AnimateDoor(false); 
        }

        public void CutPower()
        {
            _isPowerWorking = false;
            AnimateDoor(false); 
        }
    }
}

