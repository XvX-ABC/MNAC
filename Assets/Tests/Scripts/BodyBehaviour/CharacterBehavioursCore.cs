using Tests.BodyBehaviour.Arm;
using Tests.Input;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Assets.Tests.Scripts.BodyBehaviour
{
    public class CharacterBehavioursCore : BehaviourNode
    {
        protected override void Start()
        {
            this.Input = GetComponent<IInput>();
        }
        private void Update()
        {
            base.OnUpdate();
            //if (UInput.GetKeyDown(KeyCode.B))
            //{
            //    Debug.Log("Start all weapon behaviours");
            //    IArmBehaviour.TryBeginAllBehaviours(behaviours);
            //}
            //if (UInput.GetKeyDown(KeyCode.S))
            //{
            //    Debug.Log("End all weapon behaviours");
            //    IArmBehaviour.TryEndAllBehaviours(behaviours);
            //}
        }
    }
}
