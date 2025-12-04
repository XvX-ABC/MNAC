using Mono.Cecil.Cil;
using System;
using Tests.Utilities.Composable;
using Tests.Utilities.Timeline;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    internal class CaseEjectingDevice : Launcher
    {
        [SerializeField]
        ProjectilePoolSource<Case> _casePoolSource;
        IProjectilePool<Case> _casePool;
        [SerializeField]
        Vector3 _direction;
        [SerializeField, Range(0, 50)]
        float _force = 10f;
        ILauncher _launcher;

        public Vector3 Direction
        {
            get => _direction;
            set
            {
                _direction = new Vector3(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y), Mathf.Clamp01(value.z));
            }
        }
        public float Force { get => _force; set => _force = Mathf.Clamp(value, 0, 50); }
        internal ILauncher Launcher
        {
            get => _launcher;
            set
            {
                if (_launcher == value)
                    return;
                if (_launcher != null)
                    UnbindLauncher(_launcher);
                if (value != null)
                    BindLauncher(value);

                _launcher = value;
            }
        }
        public override ITimeline DelayLaunchTimeline => throw new NotImplementedException();
        public override ITimeline ReloadTimeline => throw new NotImplementedException();
        public override ITimeline LaunchingIntervalTimeline => throw new NotImplementedException();
        public override Func<bool> FireTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Func<bool> ReloadTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected override void Awake()
        {
        }
        protected override void Start()
        {
            _casePoolSource.Initialize();
            _casePool = _casePoolSource.Pool;
        }
        private void OnValidate()
        {
            Direction = _direction;
        }
        protected override void Update()
        {
        }
        protected override IProjectile GetProjectile()
        {
            var c = _casePool.Get();
            return c;
        }

        protected override void ReleaseProjectile(IProjectile projectile)
        {
            _casePool.Release(projectile as Case);
        }
        void EjectCase(Case c)
        {
            var rb = c.Rbody;
            rb.AddForceAtPosition(this.transform.TransformDirection(_direction) * _force, c.Obj.transform.position, ForceMode.Impulse);
        }
        protected internal override IProjectile Launch()
        {
            var c = GetProjectile();
            var obj = c.Obj;
            WeaponsHelper.SynchronizeWorldTransform(obj.transform, this.transform);
            c.StartAction();
            EjectCase((Case)c);
            ammo.MagazineAmount--;
            return c;
        }
        void WhenOwnerLauncherLaunch(ILauncher owner)
        {
            if (!this.enabled)
                return;
            Launch();
        }
        void WhenOwnerLauncherReload(ILauncher owner)
        {
            if (!this.enabled)
                return;
            Reload();
        }
        void BindLauncher(ILauncher launcher)
        {
            this.ammo.MagazineAmount = launcher.MagazineAmmoAmount;
            this.ammo.ReserveAmount = launcher.ReserveAmmoAmount;
            launcher.LaunchedCallback += WhenOwnerLauncherLaunch;
            launcher.ReloadCallback += WhenOwnerLauncherReload;
            this.definitions = launcher.Definitions;
        }
        void UnbindLauncher(ILauncher launcher)
        {
            launcher.LaunchedCallback -= WhenOwnerLauncherLaunch;
            launcher.ReloadCallback -= WhenOwnerLauncherReload;
        }
    }
}
