using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Arms.Actions
{
    public class ArmLocomotion : MonoBehaviour
    {
        [SerializeField]
        Target _target;
        [SerializeField]
        GameObject _body;
        [SerializeField]
        GameObject _shoulder;
        [SerializeField]
        GameObject _shoulder_end;
        IArmComponent[] _components;
        public ITarget Target
        {
            get => _target;
            set
            {
                _target = (Target)value;
                UpdateTargetForComponents(value);
            }
        }
        void Awake()
        {
            var list = new List<IArmComponent>();
            var sr = _shoulder.AddComponent<ShoulderRotation>();
            sr.ShoulderEnd = _shoulder_end;
            sr.Body = _body;
            list.Add(sr);

            _components = list.ToArray();

            Target = _target;

        }
        void UpdateTargetForComponents(ITarget target)
        {
            foreach (var c in _components)
                c.Target = target;
        }
    }
}
