using System;
using Tests.Utilities.Timeline;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    internal class CaseEjectingDevice : LauncherBase, ILauncherEffector
    {
        [Obsolete]
        GameObject _definitionsObj;
        ILauncher_Obsolete _owner;
        [SerializeField]
        Vector3 _direction;
        [SerializeField, Range(0, 50)]
        float _force = 10f;
        internal new ICaseEjectingDeviceDefinitions definitions { get => (ICaseEjectingDeviceDefinitions)base.definitions; }

        public ILauncher_Obsolete Owner => _owner;
        public void Initialize(ILauncher_Obsolete owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _owner.LaunchAction += FollowOwnerLaunch;
            base.definitions = _owner.Definitions;
            var origin = definitions.CaseOrigin;
            if (!origin.TryGetComponent<Rigidbody>(out _))
                throw new ComponentCantFindException(origin, typeof(Rigidbody));
        }
        public void Dispose()
        {
            _owner.LaunchAction -= FollowOwnerLaunch;
        }
        protected override void Awake()
        {
            actionsLock = new();
            //base.definitions = _definitionsObj.GetComponent<ICaseEjectingDeviceDefinitions>() ?? throw new ComponentCantFindException(_definitionsObj, typeof(Obsolete_MachineGunDefinitions));
            //var origin = definitions.CaseOrigin;
            //if (!origin.TryGetComponent<Rigidbody>(out _))
            //    throw new ComponentCantFindException(origin, typeof(Rigidbody));
        }
        private void OnValidate()
        {
            _direction.x = Mathf.Clamp01(_direction.x);
            _direction.y = Mathf.Clamp01(_direction.y);
            _direction.z = Mathf.Clamp01(_direction.z);
        }
        protected override ITimeline CreateDelayLaunchTimeline()
        {
            return null;
        }
        protected override ITimeline CreateReloadTimeline()
        {
            return null;
        }
        protected override GameObject CreateAmmo()
        {
            var origin = definitions.CaseOrigin;
            var obj = Instantiate(origin, this.transform);
            obj.name = origin.name + "_" + ammoPool.CountAll;
            obj.SetActive(false);
            if (obj.TryGetComponent<IEjectable>(out var e))
            {
                e.SurvivalDurationEndAction += ReleaseAmmo;
            }
            return obj;
        }
        protected override void GetAmmo(GameObject obj)
        {
            obj.transform.position = this.transform.position;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.SetParent(null);
            obj.SetActive(true);
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.Enabled = true;
        }
        protected override void ReleaseAmmo(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
        internal virtual void EjectCase(GameObject obj)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddForceAtPosition(this.transform.TransformDirection(_direction) * _force, obj.transform.position, ForceMode.Impulse);
        }
        internal override void Launch()
        {
            var obj = ammoPool.Get();
            EjectCase(obj);
            ammoInMagazineQuantity--;
            launchAction?.Invoke(this);
        }
        public override bool StartLaunch()
        {
            if (!enabled || actionsLock.StartLaunchLocked())
                return false;
            if (ammoInMagazineQuantity <= 0)
                return false;
            Launch();
            launchDurationTimeline.Restart();
            return true;
        }
        public override bool EndLaunch()
        {
            if (!enabled || actionsLock.EndLaunchLocked())
                return false;
            if (launchDurationTimeline.IsRunning)
                launchDurationTimeline.Pause();
            return true;
        }

        void FollowOwnerLaunch(ILauncher_Obsolete owner)
        {
            this.Launch();
        }
    }
}
