using UnityEngine;

namespace Tests.Players
{
    internal class PlayerCamera : MonoBehaviour
    {
        static PlayerCamera _instance;
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
