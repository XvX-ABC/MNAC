using Assets.Scripts.Utilities.Timeline;
using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{

    public interface IArmedWeaponArmAnimator
    {
        //public Playable PlayablePart { get; }
        public Playable GetPlayablePart(PlayableGraph graph);
        public IOutputSetting OutputSetting { set; }
    }
    internal interface IArmedWeaponArmBehaviour : IArmBehaviour
    {
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IArmedWeaponArmAnimator Animator { get; }

    }
}
