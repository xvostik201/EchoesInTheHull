using System;
using DG.Tweening.Core.Easing;
using Echoes.Core;
using Echoes.Equipment;
using UnityEngine;

namespace Echoes.Player
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private GameObject _flashlightObject; 
        [SerializeField] private GameObject _tabletObject;    
    
        [SerializeField] private Flashlight _flashlight;
        [SerializeField] private Tablet _tablet;
    
        public static EquipmentManager Instance {get; private set;}

        public Tablet Tablet => _tablet;
        public Flashlight Flashlight => _flashlight;
        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            InputManager.Instance.OnSlot1 += EquipFlashlight;
            InputManager.Instance.OnSlot2 += EquipTablet;
        }

        public void EquipFlashlight()
        {
            if (!_flashlight.HasCollected) return;

            _flashlightObject.SetActive(true);
            _tabletObject.SetActive(false);
        
            CursorManager.Instance.SetCursorState(false);
        }

        public void EquipTablet()
        {
            if (!_tablet.HasCollected) return;

            _tabletObject.SetActive(true);
            _flashlightObject.SetActive(false); 
        }
    }
}