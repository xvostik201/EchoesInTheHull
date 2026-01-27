using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echoes.Systems
{
    public class CCTVCamera : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private string _cameraName = "Team module";
        public string CameraName => _cameraName;

        private void Awake()
        {
            _cam.enabled = false;
        }

        public void SwitchActiveCamera(bool isActive)
        {
            _cam.enabled = isActive;
        }
    }
}
