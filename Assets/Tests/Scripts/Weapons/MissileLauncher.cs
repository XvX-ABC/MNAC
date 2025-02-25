using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Tests.Scripts.Weapons.MVC;
using FoundationStone.UI.Tests.MVC;
using System;
using Tests;
using Tests.Weapons;
using Tests.Weapons.Projectiles;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MissileLauncher : LauncherBase, IMissileLauncher
    {
        protected GameObject missileObj;
        protected IMissile missile;
        private ITimeline delayLaunchTimeline;
        public ITarget Target;
        private Action<IMissileLauncher, ITarget> targetChangedAction;
        ITarget IMissileLauncher.Target
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
            get => (IMissileLauncherDefinitions)definition;
        }
        public ITimeline DelayLaunchTimeline { get => delayLaunchTimeline; }
        public Action<IMissileLauncher, ITarget> TargetChangeAction { get => targetChangedAction; set => targetChangedAction = value; }

        protected override void Awake()
        {
            definition = GetComponent<IMissileLauncherDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IMissileLauncherDefinitions));
            if (!Definitions.AmmoOrigin.TryGetComponent<IMissile>(out _))
                throw new ComponentCantFindException(Definitions.AmmoOrigin, typeof(IMissile));
        }
        protected override void Start()
        {
            //delayLaunchTimeline = new Timeline(Definition.LaunchDelay);
            delayLaunchTimeline = new RandomLengthTimeline(Definitions.LaunchDelay_New);
            delayLaunchTimeline.AddPointEvent(0, _ =>
            {
                missile.Target = Target;
                actionsLock.LockAll();
            });
            delayLaunchTimeline.AddPointEvent(1, _ => { DoLaunch(); actionsLock.UnlockAll(); });
            base.Start();
            Reload();
        }
        protected override void Update()
        {
            base.Update();
            if (delayLaunchTimeline.IsRunning)
                delayLaunchTimeline.OnUpdate(Time.deltaTime);
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
        protected void DoLaunch()
        {
            missileObj.transform.SetParent(null);
            var missile = this.missile;

            missile.Enabled = true;

            missileObj = null;

            ammoQuantityInMagazine--;
            lastLaunchTime = Time.time;
        }
        public override bool Launch()
        {
            if (!enabled
                || Target == null
                || actionsLock.LaunchLocked())
                return false;

            if (Time.time - lastLaunchTime <= launchingInterval
                || ammoQuantityInMagazine <= 0)
                return false;

            delayLaunchTimeline.Start();

            return true;
        }

        [RequestMapping("{c_url}/DelayLaunchTimeline/UpdateEvent/Register")]
        public void RegisterDelayLaunchTimelineUpdateEvent(IRequest<Action<float>> request, IResponse<object> response)
        {
            delayLaunchTimeline.UpdateAction += request.Data;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
    }
}
