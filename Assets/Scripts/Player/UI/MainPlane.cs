using Tests.Utilities;
using UnityEngine;

namespace Tests.Players.UI
{
    internal class MainPlane : Singleton<MainPlane>
    {
        [SerializeField]
        GameObject _panelObj;
        CanvasGroup _panelGroup;
        [SerializeField]
        GameObject _continueButtonObj;
        [SerializeField]
        KeyCode _wakeUpKey;
        GamePlayPlane _gamePlayPlane => GamePlayPlane.Instance;
        GamePlayController _gamePlayController => GamePlayController.Instance;
        public bool OnlyHide
        {
            get => _panelGroup.alpha == 0;
            set
            {
                if (value)
                {
                    HideGroup(_panelGroup);
                }
                else
                {
                    ShowGroup(_panelGroup);
                }
            }
        }
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
                    EnableContinueButton = _gamePlayController.playingStatus > PlayingStatus.Ready;
                    _gamePlayController.PauseGame();
                }
                else
                {
                    ShowGameplayGroup();
                    HideGroup(_panelGroup);
                    _gamePlayController.ResumeGame();
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

        public override void Awake()
        {
            base.Awake();
            _panelGroup = _panelObj.GetComponent<CanvasGroup>();

        }
        private void Start()
        {
            HideGameplayGroup();
        }
        void ShowGroup(CanvasGroup group)
        {
            group.alpha = 1;
            group.interactable = true;
        }
        void HideGroup(CanvasGroup group)
        {
            group.alpha = 0;
            group.interactable = false;
        }
        public void ShowGameplayGroup()
        {
            _gamePlayPlane.Hide = false;
        }
        public void HideGameplayGroup()
        {
            _gamePlayPlane.Hide = true;
        }
        private void Update()
        {
            if (_gamePlayController.playingStatus > PlayingStatus.Ready && UnityEngine.Input.GetKeyDown(_wakeUpKey))
            {
                Hide = !Hide;
            }
        }
    }
}
