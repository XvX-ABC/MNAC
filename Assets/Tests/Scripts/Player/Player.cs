using Tests.Characters;
using UnityEngine;

namespace Tests.Players
{
    internal enum PlayingStatus
    {
        Ready,
        Playing,
        Paused,
    }
    internal class Player : MonoBehaviour
    {
        static Player _instance;
        CharacterComponent[] _components;
        PlayingStatus _playingStatus;
        internal PlayingStatus playingStatus
        {
            get => _playingStatus;
            set
            {
                _playingStatus = value;
            }
        }
        internal static Player Instance { get => _instance; }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            _components = GetComponentsInChildren<CharacterComponent>();
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        private void Start()
        {
            if (_playingStatus == PlayingStatus.Ready)
                Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
        public void ShowCursor()
        {
            Cursor.visible = true;
        }
        public void HideCursor()
        {
            Cursor.visible = false;
        }
        public void PauseGame()
        {
            Time.timeScale = 0;
            ShowCursor();
            playingStatus = PlayingStatus.Paused;
        }
        public void ResumeGame()
        {
            Time.timeScale = 1;
            HideCursor();
            playingStatus = PlayingStatus.Playing;
        }
    }
}
