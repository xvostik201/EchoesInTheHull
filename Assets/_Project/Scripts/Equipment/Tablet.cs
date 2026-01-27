using System;
using System.Collections.Generic;
using UnityEngine;
using Echoes.Core;
using Echoes.Player;
using Echoes.Systems;
using UnityEngine.UI;

namespace Echoes.Equipment
{
    public class Tablet : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private Transform _playerCamera;
        [SerializeField] private float _lookDownThreshold = 40f; 
        
        [Header("Screen zones")]
        [Range(0,1f)]
        [SerializeField] private float _greenZoneHeight = 0.2f;
        
        [Header("Refs")]
        [SerializeField] private List<CCTVCamera> _cameras;

        [SerializeField] private Button _forwardButton;
        [SerializeField] private Button _backButton;

        private int _currentCam;
    
        public bool IsInteracting { get; private set; }
        public bool HasCollected { get; private set; }

        private void Start()
        {
            ActiveFirstCamera();
            
            _forwardButton.onClick.AddListener(() => SwitchCamera(true));
            _backButton.onClick.AddListener(() => SwitchCamera(false));
        }

        void Update()
        {
            if (!HasCollected) return;
            CheckLookAngle();
        }
        
        private void ActiveFirstCamera()
        {
            _cameras[0].SwitchActiveCamera(true);
        }

        private void SwitchCamera(bool forward)
        {
            _cameras[_currentCam].SwitchActiveCamera(false);

            if (forward)
            {
                _currentCam = (_currentCam + 1) % _cameras.Count;
            }
            else
            {
                _currentCam--;
        
                if (_currentCam < 0) 
                {
                    _currentCam = _cameras.Count - 1;
                }
            }

            _cameras[_currentCam].SwitchActiveCamera(true);
        }
        private void CheckLookAngle()
        {
            float xRotation = _playerCamera.localEulerAngles.x;
            if (xRotation > 180) xRotation -= 360;
    
            if (xRotation >= _lookDownThreshold)
            {
                if (!IsInteracting) ToggleInteraction(true);
            }
            else
            {
                if (IsInteracting) ToggleInteraction(false);
            }
        }
    
        private void ToggleInteraction(bool isActive)
        {
            if (IsInteracting == isActive) return;
            
            IsInteracting = isActive;
            
            CursorManager.Instance.SetCursorState(isActive);
        }
    
        public void Collect()
        {
            HasCollected = true;
            EquipmentManager.Instance.EquipTablet();
        }
        
        public bool IsMouseInExitZone()
        {
            if (!IsInteracting) return false;
            
            float mouseNormalizedY = Input.mousePosition.y / Screen.height;
            
            return mouseNormalizedY > (1f - _greenZoneHeight);
        }
        
        private void OnDisable()
        {
            ToggleInteraction(false);
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !IsInteracting) return;
    
            Gizmos.matrix = Matrix4x4.TRS(_playerCamera.position + _playerCamera.forward, _playerCamera.rotation, Vector3.one);
        
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(new Vector3(0, -_greenZoneHeight/2, 0), new Vector3(1, 1 - _greenZoneHeight, 0.1f));
    
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(new Vector3(0, 0.5f - _greenZoneHeight/2, 0), new Vector3(1, _greenZoneHeight, 0.1f));
        }
    }
}