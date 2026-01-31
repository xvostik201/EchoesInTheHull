using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Echoes.Systems
{
    public class CCTVCamera : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private string _cameraName = "Team module";
        
        [Header("Light")]
        [SerializeField] private Light _light;

        [SerializeField] private float _duration;
        [SerializeField] private float _minIntensity;
        [SerializeField] private float _maxIntensity;
        public string CameraName => _cameraName;

        private void Awake()
        {
            _cam.enabled = false;
        }

        private void Start()
        {
            _light.DOIntensity(_maxIntensity, _duration)
                .From(_minIntensity) 
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void SwitchActiveCamera(bool isActive)
        {
            _cam.enabled = isActive;
        }
    }
}
