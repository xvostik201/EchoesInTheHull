using System;
using System.Collections;
using System.Collections.Generic;
using Echoes.Core;
using Echoes.Player;
using UnityEngine;

namespace Echoes.Equipment
{
    public class Flashlight : MonoBehaviour
    {
        [Header("General settings")] [SerializeField]
        private float _maxBatteryPower = 50f;

        [SerializeField] private float _batteryConsumption = 0.5f;

        private float _batteryCapacity;

        [Header("Light")] [SerializeField] private Light _light;

        private bool _isActive = false;
        public bool HasCollected { get; private set; }

        private void Awake()
        {
            _batteryCapacity = _maxBatteryPower;
            _light.enabled = _isActive;
        }

        private void OnEnable()
        {
            InputManager.Instance.OnToggleFlashlight += HandleToggleFlashlight;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnToggleFlashlight -= HandleToggleFlashlight;
        }

        void Update()
        {
            if (!HasCollected) return;

            if (_isActive)
            {
                _batteryCapacity -= _batteryConsumption * Time.deltaTime;
                if (_batteryCapacity <= 0)
                {
                    _isActive = false;
                    _light.enabled = _isActive;
                }
            }
        }

        private void HandleToggleFlashlight()
        {
            if (!HasCollected) return;

            if (_batteryCapacity <= 0)
            {
                _isActive = false;
                _light.enabled = _isActive;
                return;
            }

            _isActive = !_isActive;
            _light.enabled = _isActive;
        }

        public void Collect()
        {
            HasCollected = true;
            EquipmentManager.Instance.EquipFlashlight();
        }
    }
}
