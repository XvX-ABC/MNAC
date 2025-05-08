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
        protected IArmBehaviour[] behaviours;

        public IInput Input
        {
            set
            {
                foreach (var b in behaviours)
                    b.Input = value;
            }
        }

        bool IArmBehaviour.Continuing => IArmBehaviour.AnyBehaviourIsContinuing(behaviours);

        protected virtual void Awake()
        {
            var length = _subBehaviourObjs.Length;
            behaviours = new IArmBehaviour[length];
            for (int i = 0; i < length; i++)
            {
                behaviours[i] = _subBehaviourObjs[i].GetComponent<IArmBehaviour>() ?? throw new ComponentCantFindException(_subBehaviourObjs[i], typeof(IArmBehaviour));
            }
        }
        protected virtual void Start()
        {

        }
        public void OnUpdate()
        {

            foreach (var b in behaviours)
                b.OnUpdate();
        }
        public void OnAnimatorIK(int layerIndex)
        {
            foreach (var b in behaviours)
                b.OnAnimatorIK(layerIndex);
        }

        bool IArmBehaviour.Begin()
        {
            return IArmBehaviour.TryBeginAllBehaviours(behaviours);
        }

        bool IArmBehaviour.End()
        {
            return IArmBehaviour.TryEndAllBehaviours(behaviours);
        }


    }
}
