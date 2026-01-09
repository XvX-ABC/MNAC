using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Players.UI
{
    [RequireComponent(typeof(Player))]
    internal class PlayerPanelController : MonoBehaviour
    {
        static PlayerPanelController _instance;
        [SerializeField]
        PlayerPanel _panel;
        Player _player => Player.Instance;
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
                    _player.PauseGame();
                else
                    _player.ResumeGame();
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
