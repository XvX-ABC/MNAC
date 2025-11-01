using Assets.Tests.Scripts.Weapons.MVC;
using FoundationStone.UI.Tests.MVC;
using System;
using Tests.Utilities.Timeline;
using Tests.Weapons.Launcher;
using Tests.Weapons.Projectiles;
using UnityEngine;
using Tests.Utilities.Timeline.Events.Point;

namespace Tests.Weapons.MissileLauncher
{
    public class MissileLauncher : LauncherBase, IMissileLauncher
    {
        protected GameObject missileObj;
        protected IMissile missile;
        //protected ITimeline delayLaunchTimeline;
        //protected ITimeline launchDurationTimeline;
        public ITarget_Obsolete Target;
        private Action<IMissileLauncher, ITarget_Obsolete> targetChangedAction;
        ITarget_Obsolete IMissileLauncher.Target
        {
            get => Target;
            set
            {
                targetChangedAction?.Invoke(this, value);
                Target = value;
            }
        }
        public new IMissileLauncherDefinitions Definitions
        {
            get => (IMissileLauncherDefinitions)definitions;
        }
        public Action<IMissileLauncher, ITarget_Obsolete> TargetChangeAction { get => targetChangedAction; set => targetChangedAction = value; }

        protected override void Awake()
        {
            definitions = GetComponent<IMissileLauncherDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IMissileLauncherDefinitions));
            if (!Definitions.AmmoOrigin.TryGetComponent<IMissile>(out _))
                throw new ComponentCantFindException(Definitions.AmmoOrigin, typeof(IMissile));
        }
        protected override void Start()
        {
            //delayLaunchTimeline = new RandomLengthTimeline(Definitions.LaunchDelayRange);
            //delayLaunchTimeline.AddPointEvent(0, _ =>
            //{
            //    missile.Target = Target;
            //    actionsLock.LockAll();
            //});
            //delayLaunchTimeline.AddPointEvent(1, _ => { DoLaunch(); actionsLock.UnlockAll(); });

            //launchDurationTimeline = new Timeline(Definitions.LaunchDurationTime);
            //launchDurationTimeline.AddPointEvent(0, _ => actionsLock.LockAll());
            //launchDurationTimeline.AddPointEvent(1, _ => actionsLock.UnlockAll());
            base.Start();
            Reload();
        }
        protected override ITimeline CreateDelayLaunchTimeline()
        {
            var timeline = new RandomLengthTimeline(definitions.LaunchDelayRange);
            timeline.AddPointEvent(0, _ =>
            {
                missile.Target = Target;
                actionsLock.LockStartLaunch();
            });
            timeline.AddPointEvent(1, _ =>
            {
                Launch();
                actionsLock.UnlockAll();
                launchDurationTimeline.Restart();
            });
            return timeline;
        }
        protected override void GetAmmo(GameObject obj)
        {
            obj.transform.localPosition = Definitions.MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);
            var missile = obj.GetComponent<IMissile>();
            missile.Enabled = false;
        }
        internal override void Reload()
        {
            base.Reload();
            if (missileObj == null)
            {
                missileObj = ammoPool.Get();
                missile = missileObj.GetComponent<IMissile>();
            }
        }
        internal override void Launch()
        {
            missileObj.transform.SetParent(null);
            var missile = this.missile;

            missile.Enabled = true;

            missileObj = null;

            ammoInMagazineQuantity--;
        }
        public override bool StartLaunch()
        {
            if (!enabled
                || Target == null
                || actionsLock.StartLaunchLocked())
                return false;

            if (ammoInMagazineQuantity <= 0)
                return false;

            delayLaunchTimeline.Restart();

            return true;
        }
    }
}
