using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using Tests.Weapons;
using UnityEngine.Analytics;
using UnityEngine.Playables;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Animation
{
    internal class ArmWeaponHoldingBehavioursAnimator
    {
        ArmWeaponHoldingBehaviours _behaviours;
        internal Playable playablePart;
        PlayableGraph _graph;
        float _weight;
        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }
        public ArmWeaponHoldingBehavioursAnimator(ArmWeaponHoldingBehaviours behaviours, PlayableGraph graph)
        {
            _behaviours = behaviours ?? throw new ArgumentNullException(nameof(behaviours));
            _behaviours.ActivatedAction += this.ActivatedAnimator;
            _behaviours.UnactivatedAction += this.UnactivatedAnimator;
            _graph = graph;
        }

        void ActivatedAnimator(IWeapon weapon, IArmWeaponHoldingBehaviour behaviour)
        {
            var animator = behaviour.Animator;
            this.playablePart = animator.GetPlayablePart(_graph);
        }
        void UnactivatedAnimator(IWeapon weapon, IArmWeaponHoldingBehaviour behaviour)
        {
            var p = behaviour.Animator.GetPlayablePart(_graph);
            if (p.Equals(this.playablePart))
                this.playablePart = default;
            return;
        }
    }
}
