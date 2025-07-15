using Assets.Scripts.Utilities.Timeline;
using System;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{

    public interface IArmWeaponHoldingBehavioursAnimator
    {
        //public Playable PlayablePart { get; }
        public Playable GetPlayablePart(PlayableGraph graph);
    }
    internal interface IArmWeaponHoldingBehaviour : IArmBehaviour
    {
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IArmWeaponHoldingBehavioursAnimator Animator { get; }

    }
}
