using UnityEngine;

namespace Tests.Players.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    internal class GamePlayPlane : MonoBehaviour
    {
        public static GamePlayPlane Instance;

        CanvasGroup _group;
        public bool Hide
        {
            get => _group.alpha == 0;
            set
            {
                _group.alpha = value ? 0 : 1;
                _group.interactable = !value;
            }
        }
        void Awake()
        {
            Instance = this;
            _group = GetComponent<CanvasGroup>();
        }
    }
}
