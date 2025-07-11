using Assets.Scripts.Utilities.Timeline;
using System;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public interface IArmWeaponHoldingBehavioursAnimator
    {
        public Animator Animator { get; set; }
        public void OnAwake();
        public void OnEnter();
        public void OnExit();
        public void OnUpdate();
    }
    internal class BehaviourTransition
    {
        protected ITimeline timeline;
        public void Play()
        {

        }
    }
    internal interface IArmWeaponHoldingBehaviour : IArmBehaviour
    {
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IArmWeaponHoldingBehavioursAnimator Animator { get; set; }

    }
}
