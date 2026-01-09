using Mono.Cecil.Cil;
using NUnit.Framework.Interfaces;
using Tests.Utilities;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Tests.Players.UI
{
    internal class PlayerPanel : MonoBehaviour
    {
        static PlayerPanel s_instance;
        [SerializeField]
        CanvasGroup _gameplayPanel;
        [SerializeField]
        GameObject _panelObj;
        CanvasGroup _panelGroup;
        [SerializeField]
        GameObject _continueButtonObj;
        [SerializeField]
        KeyCode _wakeUpKey;
        Player _player => Player.Instance;
        public bool Hide
        {
            get => _panelGroup.alpha == 0;
            set
            {
                var v = !value;
                if (v)
                {
                    HideGameplayGroup();
                    ShowGroup(_panelGroup);
                    EnableContinueButton = _player.playingStatus > PlayingStatus.Ready;
                    _player.PauseGame();
                }
                else
                {
                    ShowGameplayGroup();
                    HideGroup(_panelGroup);
                    _player.ResumeGame();
                }

            }
        }
        public bool EnableContinueButton
        {
            get => _continueButtonObj.gameObject.activeSelf;
            set
            {
                _continueButtonObj.gameObject.SetActive(value);
            }
        }

        internal static PlayerPanel instance { get => s_instance; }

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            GameObject.DontDestroyOnLoad(this.gameObject);
            s_instance = this;

            _panelGroup = _panelObj.GetComponent<CanvasGroup>();

            //HideGameplayGroup();
        }
        void ShowGroup(CanvasGroup group)
        {
            Debug.Log(DebuggingHelper.GetCurrentCallerInfo());
            group.alpha = 1;
            group.interactable = true;
        }
        void HideGroup(CanvasGroup group)
        {
            Debug.Log(DebuggingHelper.GetCurrentCallerInfo());
            group.alpha = 0;
            group.interactable = false;
        }
        void ShowGameplayGroup()
        {
            if (_gameplayPanel != null)
                ShowGroup(_gameplayPanel);
        }
        void HideGameplayGroup()
        {
            if (_gameplayPanel != null)
                HideGroup(_gameplayPanel);
        }
        private void Update()
        {
            if (_player.playingStatus > PlayingStatus.Ready && UnityEngine.Input.GetKeyDown(_wakeUpKey))
            {
                Hide = !Hide;
            }
        }
    }
}
