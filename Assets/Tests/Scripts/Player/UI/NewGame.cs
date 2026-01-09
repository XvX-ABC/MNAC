using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Tests.Players.UI
{
    internal class NewGame : MonoBehaviour
    {
        PlayerPanel _panel => PlayerPanel.instance;
        Player _player => Player.Instance;
        void OnEnable()
        {
            SceneManager.sceneLoaded += WhenSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= WhenSceneLoaded;
        }
        public void Execute()
        {
            if (_player.playingStatus > PlayingStatus.Ready)
            {
                Transitioner.Instance.enabled = true;
                Transitioner.Instance.TransitionToScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                _player.playingStatus = PlayingStatus.Playing;
            }
            _panel.Hide = true;
        }
        void WhenSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Transitioner.Instance.enabled = false;
        }
    }
}
