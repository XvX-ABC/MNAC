using Locomotion.Animation;
using System;
using Tests.Environment;
using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace Tests.Locomotion.Animation
{
    public class GroundLocomotionAnimator_New : MonoBehaviour, IModule
    {
        const float BottomHeight = 0.8f;
        [SerializeField]
        TwoBoneIKConstraint _constraint;
        IGroundLocomotionAnimatorDefinitions _definitions;
        IBonesDefinitions _bonesDefinitions;
        Animator _animator;
 

        void Awake()
        {
            _definitions = GetComponent<IGroundLocomotionAnimatorDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundLocomotionAnimatorDefinitions));
            _bonesDefinitions = GetComponent<IBonesDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IBonesDefinitions));
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
        }

        public void OnFixedUpdate(Context context)
        {
            throw new NotImplementedException();
        }

        //(Vector3,Quaternion) CalculateFootIKPosAndRotation(ushort legNum)
        //{
        //    var bottomHeight = BottomHeight;
        //}
    }
}