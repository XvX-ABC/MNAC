using Assets.Scripts.Utilities.Timeline;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class AmmoLoad : ArmedArmStateBase
    {
        public ITimeline ReloadTimeline
        {
            set
            {
                this.timeline = value;
            }
        }
        public AmmoLoad() : base("arm_launcher_ammo_load", 0)
        {
        }
        public override void OnEnter()
        {
            timeline.Start();
        }
        public override void OnExit()
        {
            timeline.Stop();
        }
        public override void OnUpdate()
        {
            timeline.OnUpdate(Time.deltaTime);
        }
    }
}
