using UnityEngine;
namespace Tests.Input
{
    public class HybridInput : MonoBehaviour, IHybridInput
    {
        [SerializeField]
        CustomPlayerInput _playerInput;
        VirtualInput _virtualInput;
        IHybridInput.Mode _mode;


        IInput_Obsolete _currentInput;
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
        public Vector3 HorizontalVector
        {
            get => _currentInput.HorizontalVector;
            set => _virtualInput.HorizontalVector = value;
        }
        public bool Jump
        {
            get => _currentInput.Jump;
            set => _virtualInput.Jump = value;
        }
        public bool Boost
        {
            get => _currentInput.Boost;
            set => _virtualInput.Boost = value;
        }
        public bool QuickBoost
        {
            get => _currentInput.QuickBoost;
            set => _virtualInput.QuickBoost = value;
        }

        public bool Fire => _currentInput.Fire;

        public bool Reload => _currentInput.Reload;

        public bool Supply => _currentInput.Supply;
    }
}