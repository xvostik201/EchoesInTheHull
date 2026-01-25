using System.Collections;
using System.Collections.Generic;
using Echoes.Core;
using Echoes.Equipment;
using UnityEngine;

namespace Echoes.Interactions
{
    public class FlashlightPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private Flashlight _playerFlashlight;
    
        public void Interact()
        {
            if (_playerFlashlight != null)
            {
                _playerFlashlight.Collect();
            
                gameObject.SetActive(false);
            
                Debug.Log("Flashlight collected");
            }
        }
    }
}
