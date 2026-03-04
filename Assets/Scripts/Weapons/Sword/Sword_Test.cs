using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal class Sword_Test : MonoBehaviour
    {
        [SerializeField]
        Animator _animator;
        [SerializeField]
        string _slashMultiplier;
        [SerializeField]
        float _slashDuration;
        [SerializeField]
        KeyCode _playKey;
        [SerializeField]
        bool _enableSlashAction;
        [SerializeField]
        bool _enableExtensionAction;
        [SerializeField]
        string _clipName;
        [SerializeField]
        Sword _sword;

        SwordAction _slashAction;
        SwordAction _extensionAction;

        GUIStyle _style;
        private void Start()
        {
            _sword.HitAction += WhenHItObj;
            _slashAction = _sword.GetSwordAction(SwordActionType.Slash);
            _extensionAction = _sword.GetSwordAction(SwordActionType.Extension);
            _style = new GUIStyle();
            _style.normal.textColor = Color.black;

            _slashAction.Enabled = _enableSlashAction;

            var v = 2.458f / _slashDuration;
            _animator.SetFloat(_slashMultiplier, v);
        }
        void WhenHItObj(GameObject obj)
        {
            Debug.Log($"hit obj '{obj.name}'");
        }
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_playKey))
            {
                _animator.Play(_clipName, 0, 0);
            }
            //if (_stayExtended == _cache)
            //    return;
            //if (_stayExtended)
            //{
            //    _animationControl.Play(1);
            //    _cache = _stayExtended;
            //}
            //else
            //{
            //    _animationControl.Play(0);
            //    _cache = _stayExtended;
            //}
        }
        private void OnValidate()
        {
            if (_slashAction != null && _slashAction.Enabled != _enableSlashAction)
                _slashAction.Enabled = _enableSlashAction;
            if (_extensionAction != null && _extensionAction.Enabled != _enableExtensionAction)
                _extensionAction.Enabled = _enableExtensionAction;
        }
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"slash action state: {_slashAction.Enabled}", _style);
            GUILayout.Label($"sword current action: {_sword.currentAction?.Type.ToString() ?? ""}", _style);
            GUILayout.EndVertical();
        }
    }
}
