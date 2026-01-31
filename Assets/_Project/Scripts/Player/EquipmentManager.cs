using System;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using Echoes.Core;
using Echoes.Equipment;
using UnityEngine;

namespace Echoes.Player
{
    public class EquipmentManager : MonoBehaviour
    {
        [Header("Equipment")]
        [SerializeField] private List<GameObject> _equipmentList;
        
        [Header("Equipment objects")]
        [SerializeField] private GameObject _flashlightObject; 
        [SerializeField] private GameObject _tabletObject;    
    
        [Header("Equipment components")]
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
            InputManager.Instance.OnHideAll += HandleHideAll;
        }

        public void EquipFlashlight()
        {
            if (!_flashlight.HasCollected) return;

            HandleHideAll();
            
            _flashlightObject.SetActive(true);
        
            CursorManager.Instance.SetCursorState(false);
        }

        public void EquipTablet()
        {
            if (!_tablet.HasCollected) return;

            HandleHideAll();

            _tabletObject.gameObject.SetActive(true);
        }

        private void HandleHideAll()
        {
            foreach (GameObject equipment in _equipmentList)
            {
                equipment.SetActive(false);
            }
        }
    }
}