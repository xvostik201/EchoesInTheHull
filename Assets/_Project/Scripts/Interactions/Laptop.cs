using System;
using UnityEngine;
using TMPro;
using Echoes.Core;
using Echoes.Player;

namespace Echoes.Interactions
{
    public class Laptop : MonoBehaviour, IInteractable
    {
        [Header("General settings")] [SerializeField]
        private string _password = "eagle11";

        [SerializeField, TextArea(10, 10)] private string _noteText;

        [Header("Note animation")] [SerializeField]
        private float _minYClamp;

        [SerializeField] private float _maxYClamp;

        [SerializeField] private float _scrollSpeed = 0.01f;

        [Header("TMP components")] [SerializeField]
        private TMP_Text _noteTMPtext;

        [SerializeField] private TMP_InputField _inputField;

        [Header("Refs")] [SerializeField] private PlayerController _playerController;

        private bool _isUsingLaptop = false;
        private bool _isEntered = false;

        private void Awake()
        {
            _inputField.onEndEdit.AddListener(delegate { CheckPassword(); });
        }

        private void OnEnable()
        {
            InputManager.Instance.OnExitLaptop += HandleExitLaptop;
            InputManager.Instance.OnScroll += HandleScrollText;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnExitLaptop -= HandleExitLaptop;
            InputManager.Instance.OnScroll += HandleScrollText;
        }

        private void HandleScrollText(float speed)
        {
            if (_isUsingLaptop && _isEntered)
            {
                float normalizedSpeed = Mathf.Sign(speed); 

                if (Mathf.Abs(speed) > 0.01f)
                {
                    RectTransform rect = _noteTMPtext.GetComponent<RectTransform>();

                    float newY = rect.anchoredPosition.y + (normalizedSpeed * _scrollSpeed);

                    newY = Mathf.Clamp(newY, _minYClamp, _maxYClamp);
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, newY);
                }
            }
        }

        private void HandleExitLaptop()
        {
            if (_isUsingLaptop)
            {
                ExitLaptop();
            }
        }

        public void Interact()
        {
            if (!_isUsingLaptop)
            {
                EnterLaptop();
            }
        }

        private void EnterLaptop()
        {
            _isUsingLaptop = true;

            _inputField.ActivateInputField();

            CursorManager.Instance.SetCursorState(true);

            _playerController.enabled = false;
        }

        private void ExitLaptop()
        {
            _isUsingLaptop = false;

            _playerController.enabled = true;

            CursorManager.Instance.SetCursorState(false);
        }

        private void CheckPassword()
        {
            if (string.IsNullOrEmpty(_inputField.text)) return;

            if (_inputField.text.ToLower() == _password)
            {
                ShowNote();
                _isEntered = true;
            }
            else
            {
                _inputField.text = "";
                _inputField.placeholder.GetComponent<TMP_Text>().text = "ACCESS DENIED...";
                _inputField.ActivateInputField();
            }
        }

        private void ShowNote()
        {
            _inputField.gameObject.SetActive(false);
            _noteTMPtext.text = _noteText;
            _noteTMPtext.gameObject.SetActive(true);
        }
    }
}