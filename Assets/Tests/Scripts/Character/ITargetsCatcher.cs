using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using UnityEngine;

namespace Tests.Characters
{
    public interface ITargetsCatcher
    {
        public Blackboard Blackboard { get; set; }
        public IList<ITarget> Targets { get; }
        public Action<IList<ITarget>> TargetsChangedAction { get; set; }
    }
    [Serializable]
    public class TargetsCatcher_Debug : ITargetsCatcher
    {
        Blackboard _blackboard;
        List<ITarget> _targets;
        Action<IList<ITarget>> _targetsChangedAction;
        [SerializeField]
        Target _target;

        public Blackboard Blackboard
        {
            get => _blackboard;
            set
            {
                _blackboard = value;
            }
        }
        public IList<ITarget> Targets { get => _targets; }
        public Action<IList<ITarget>> TargetsChangedAction { get => _targetsChangedAction; set => _targetsChangedAction = value; }

        public void OnAwake()
        {
            _targets = new();
            _targets.Add(_target);
        }
        public void OnUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                if (_targets.Contains(_target))
                    _targets.Remove(_target);
                else
                    _targets.Add(_target);
                _targetsChangedAction?.Invoke(_targets);
            }
        }
    }
}
