using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.BodyBehaviour.Arm;
using Tests.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Tests.Scripts.BodyBehaviour
{
    public class BehaviourNode : MonoBehaviour, IArmBehaviour
    {
        [SerializeField]
        GameObject[] _subBehaviourObjs;
        IArmBehaviour[] _behaviours;

        IInput IArmBehaviour.Input
        {
            set
            {
                foreach (var b in _behaviours)
                    b.Input = value;
            }
        }

        bool IArmBehaviour.Continuing => IArmBehaviour.AnyBehaviourIsContinuing(_behaviours);

        protected void Awake()
        {
            var length = _subBehaviourObjs.Length;
            _behaviours = new IArmBehaviour[length];
            for (int i = 0; i < length; i++)
            {
                _behaviours[i] = _subBehaviourObjs[i].GetComponent<IArmBehaviour>() ?? throw new ComponentCantFindException(_subBehaviourObjs[i], typeof(IArmBehaviour));
            }
        }
        public void Update()
        {
  
            foreach (var b in _behaviours)
                b.Update();
        }
        public void OnAnimatorIK(int layerIndex)
        {
            foreach (var b in _behaviours)
                b.OnAnimatorIK(layerIndex);
        }

        bool IArmBehaviour.Begin()
        {
            return IArmBehaviour.TryBeginAllBehaviours(_behaviours);
        }

        bool IArmBehaviour.End()
        {
            return IArmBehaviour.TryEndAllBehaviours(_behaviours);
        }


    }
}
