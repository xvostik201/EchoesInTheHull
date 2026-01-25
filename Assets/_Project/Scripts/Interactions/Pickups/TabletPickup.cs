using System.Collections;
using System.Collections.Generic;
using Echoes.Core;
using Echoes.Equipment;
using UnityEngine;

namespace Echoes.Interactions
{
    public class TabletPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private Tablet _playerTablet;
    
        public void Interact()
        {
            if (_playerTablet != null)
            {
                _playerTablet.Collect();
            
                gameObject.SetActive(false);
                Debug.Log("Tablet collected");
            }
        }
    }
}
