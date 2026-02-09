using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum LampState
{
    Stable,
    Flickering
}

public class Lamp : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private LampState _state = LampState.Stable;
    
    [Header("General Settings")]
    [SerializeField] private Light[] _lights;
    [SerializeField] private bool _isDebbuging = false;
    

    [Header("Stable Settings")] 
    [SerializeField] private float _minIntensity = 0.85f;
    [SerializeField] private float _stableAnimDuration = 1f;

    [Header("Flickering Settings")] 
    [SerializeField, TextArea(5,5)] private string _pattern = "mmmammmammmaaaaaammm";
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private float _smoothness = 0.05f;
    [SerializeField] private float _maxIntensity = 1.1f;
    
    private int _currentIndex = 0;
    private float _timer;

    private void Awake()
    {
    }

    private void Start()
    {
        if (_lights == null || _lights.Length == 0)
        {
            Debug.LogError("No lights found!" +
                           "Check ARRAY! ");
            return;
        }
        
        ChangeState(_state);
    }

    private IEnumerator Stable()
    {
        foreach (Light light in _lights)
        {
            light.DOIntensity(_minIntensity, _stableAnimDuration).
                SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        yield break;
    }
    
    private IEnumerator Flickering()
    {
        while (true)
        {
            char c = _pattern[_currentIndex];

            float brightness01 = (c - 'a') / 25f;
            float intensivity =  brightness01 * _maxIntensity;

            foreach (Light light in _lights)
            {
                light.DOIntensity(intensivity, _smoothness);
            }
            
            _currentIndex = (_currentIndex + 1) % _pattern.Length;
            
            yield return new WaitForSeconds(_speed);
        }
    }

    public void ChangeState(LampState newState)
    {
        StopAllCoroutines();
        _state = newState; 

        foreach (var l in _lights) l.DOKill(); 

        switch (newState)
        {
            case LampState.Stable:
                StartCoroutine(Stable());
                break;
            case LampState.Flickering:
                _currentIndex = 0;
                StartCoroutine(Flickering());
                break;
        }
    }
    
    private void OnGUI()
    {
        if (!_isDebbuging)
            return;
        Rect rect = new Rect(20, 20, 300, 100);
    
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
    
        GUIStyle shadowStyle = new GUIStyle(style);
        shadowStyle.normal.textColor = Color.black;
    
        string info = $"LAMP DEBUG\nState: {_state}\nCurrent Char: {_pattern[_currentIndex]}";

        GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), info, shadowStyle);
        GUI.Label(rect, info, style);
    }
}
