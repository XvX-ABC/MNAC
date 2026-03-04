using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace MNAC.Players.UI
{
    internal class NewGame : MonoBehaviour
    {
        MainPlane _mainPanel => MainPlane.Instance;
        GamePlayPlane _gamePlayPlane => GamePlayPlane.Instance;
        GamePlayController _gamePlayController => GamePlayController.Instance;
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
            if (_gamePlayController.playingStatus > PlayingStatus.Ready)
            {
                _mainPanel.Hide = true;
                _gamePlayPlane.Hide = true;
                Transitioner.Instance.enabled = true;
                Transitioner.Instance.TransitionToScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                _gamePlayController.playingStatus = PlayingStatus.Playing;
                _mainPanel.Hide = true;
            }


        }
        void WhenSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Transitioner.Instance.enabled = false;
        }
    }
}
