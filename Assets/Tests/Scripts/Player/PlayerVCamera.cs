using UnityEngine;

namespace Tests.Players
{
    internal class PlayerVCamera : MonoBehaviour
    {
        static PlayerVCamera _instance;
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }
}
