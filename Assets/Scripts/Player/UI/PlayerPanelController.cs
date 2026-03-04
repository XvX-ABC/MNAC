using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MNAC.Players.UI
{
    [RequireComponent(typeof(GamePlayController))]
    internal class PlayerPanelController : MonoBehaviour
    {
        static PlayerPanelController _instance;
        [SerializeField]
        MainPlane _panel;
        GamePlayController _gamePlayController => GamePlayController.Instance;
        [SerializeField]
        KeyCode _wakeUpKey;
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            GameObject.DontDestroyOnLoad(_panel.gameObject);
            _instance = this;

        }
        public bool EnablePanel
        {
            get => _panel.gameObject.activeSelf;
            set
            {
                _panel.gameObject.SetActive(value);
                if (value)
                    _gamePlayController.PauseGame();
                else
                    _gamePlayController.ResumeGame();
            }
        }
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_wakeUpKey))
            {
                var isWakeUp = !_panel.gameObject.activeSelf;
                EnablePanel = isWakeUp;
            }
        }
    }
}
