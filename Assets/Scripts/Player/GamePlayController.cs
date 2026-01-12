using Tests.Characters;
using Tests.Utilities;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Players
{
    internal enum PlayingStatus
    {
        Ready,
        Playing,
        Paused,
    }
    internal class GamePlayController : Singleton<GamePlayController>
    {
        CharacterComponent[] _components;
        [SerializeField]
        PlayingStatus _playingStatus;
        internal PlayingStatus playingStatus
        {
            get => _playingStatus;
            set
            {
                _playingStatus = value;
            }
        }

        public override void Awake()
        {
            if (Instance == null)
                _playingStatus = PlayingStatus.Ready;
            base.Awake();
            _components = GetComponentsInChildren<CharacterComponent>();

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
