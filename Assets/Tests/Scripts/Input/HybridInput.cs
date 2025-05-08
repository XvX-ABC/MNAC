using UnityEngine;
namespace Tests.Input
{
    public class HybridInput : MonoBehaviour, IHybridInput
    {
        [SerializeField]
        CustomPlayerInput _playerInput;
        VirtualInput _virtualInput;
        IHybridInput.Mode _mode;


        IInput _currentInput;
        public HybridInput()
        {
            _virtualInput = new();
        }
        void Awake()
        {
            _currentInput = _playerInput;
        }
        public IHybridInput.Mode CurrentMode
        {
            get => _mode;
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case IHybridInput.Mode.Player:
                        _currentInput = _playerInput;
                        break;
                    case IHybridInput.Mode.Virtual:
                        _currentInput = _virtualInput;
                        break;
                }
            }
        }
        public Vector3 HorizontalDirection
        {
            get => _currentInput.HorizontalDirection;
            set => _virtualInput.HorizontalDirection = value;
        }
        public bool IsAscending
        {
            get => _currentInput.IsAscending;
            set => _virtualInput.IsAscending = value;
        }
        public bool IsBoosting
        {
            get => _currentInput.IsBoosting;
            set => _virtualInput.IsBoosting = value;
        }

        public bool Fire => false;

        public bool Reload => false;

        public bool Supply => false;
    }
}