using System;
using System.Collections;
using System.Collections.Generic;
using Echoes.Interactions;
using UnityEngine;

namespace Echoes.Systems
{
    public class PowerManager : MonoBehaviour
    {
        [Header("General settings")] [SerializeField]
        private Button _restartButton;

        [SerializeField] private List<Door> _doors = new List<Door>();

        private void OnEnable()
        {
            _restartButton.OnButtonPressed += RestartDoors;
        }

        private void OnDisable()
        {
            _restartButton.OnButtonPressed -= RestartDoors;
        }

        private void RestartDoors()
        {
            foreach (var door in _doors)
            {
                door.Restart();
            }
        }
    }
}
