using UnityEngine;

namespace Echoes.Core
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        public bool IsCursorVisible => Cursor.visible;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetCursorState(false);
        }

        public void SetCursorState(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}

