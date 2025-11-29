using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Timeline;
using UnityEngine;
using IProjectile = Tests.Weapons_New.Projectiles.IProjectile;

namespace Tests.Weapons_New.Launcher
{
    //DONE: 使用状态机重写发射器逻辑
    internal abstract class Launcher : Weapon, ILauncher
    {
        protected struct Ammo
        {
            public int MagazineAmount;
            public int ReserveAmount;
        }
        protected ILauncherDefinitions definitions;
        [SerializeField]
        [Obsolete]
        Transform _magazineObj;
        [SerializeField]
        Transform _muzzleTrans;
        [SerializeField]
        bool _launchWhenEnter;
        [SerializeField]
        LauncherComponent[] _subComponents;
        Blackboard _blackboard;

        protected LauncherStatemachine statemachine;
        protected Idle idle;
        protected DelayLaunch delayLaunch;
        protected Launching launching;
        protected Reload reload;

        protected Func<bool> fireTrigger;
        protected Func<bool> reloadTrigger;

        protected Ammo ammo;

        protected Action<ILauncher> _launchedCallback;
        protected Action<ILauncher> _reloadCallback;

        public virtual ITimeline DelayLaunchTimeline { get => delayLaunch.Timeline; }
        public virtual ITimeline ReloadTimeline { get => reload.Timeline; }
        public virtual ITimeline LaunchingIntervalTimeline { get => launching.intervalTimeline; }

        public Action<ILauncher> LaunchedCallback { get => _launchedCallback; set => _launchedCallback = value; }
        public Action<ILauncher> ReloadCallback { get => _reloadCallback; set => _reloadCallback = value; }

        //public string Name { get => name; }
        public ushort ReserveAmmoAmount { get => (ushort)ammo.ReserveAmount; }
        public ushort MagazineAmmoAmount { get => (ushort)ammo.MagazineAmount; }
        public ILauncherDefinitions Definitions { get => definitions; }
        public override WeaponType Type { get => WeaponType.Launcher; }
        public virtual Func<bool> FireTrigger { get => fireTrigger; set => fireTrigger = value; }
        public virtual Func<bool> ReloadTrigger { get => reloadTrigger; set => reloadTrigger = value; }
        public Transform MuzzleTrans { get => _muzzleTrans; }


        //public GameObject Obj => this.gameObject;

        protected virtual void Awake()
        {
            definitions = GetDefinitions();
            InitializeStatemachine();
            ammo = new()
            {
                MagazineAmount = definitions.AmmoInMagazineAmount,
                ReserveAmount = definitions.AmmoTotalAmount - definitions.AmmoInMagazineAmount
            };
            _blackboard = new();

        }
        protected virtual void Start()
        {
            _blackboard.TryRegisterField(LauncherComponent.OwnerLauncher, this);
            foreach (var comp in _subComponents)
                comp?.Initialize(_blackboard);
        }
        protected virtual void OnEnable()
        {

        }
        protected virtual void OnDisable()
        {
            foreach (var comp in _subComponents)
                comp?.Dispose();
            _blackboard.TryUnregisterField(LauncherComponent.OwnerLauncher);
        }
        protected virtual void Update()
        {
            statemachine.OnUpdate();
        }
        protected virtual ILauncherDefinitions GetDefinitions()
        {
            return GetComponent<ILauncherDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ILauncherDefinitions));
        }
        protected virtual void InitializeAmmoObj()
        {
            ammo = new()
            {
                MagazineAmount = definitions.AmmoInMagazineAmount,
                ReserveAmount = definitions.AmmoTotalAmount - definitions.AmmoInMagazineAmount
            };
        }
        protected virtual void InitializeStatemachine()
        {
            var name = this.name;
            idle = new(this, name);
            delayLaunch = new(this, name, definitions.LaunchDelayRange);
            launching = new(this, name, definitions.LaunchingIntervalTime);
            reload = new(this, name, 0.8f, definitions.ReloadDurationTime);
            statemachine = new(name);
            statemachine.AddState(idle);
            statemachine.AddState(delayLaunch);
            statemachine.AddState(launching);
            statemachine.AddState(reload);


            var i_d = new Transition(idle, delayLaunch, HoldingFire, null, 0);
            var i_r = new Transition(idle, reload, CanReload, null, 0);
            statemachine.AddTransitionFor(i_d);
            statemachine.AddTransitionFor(i_r);

            var d_l = new Transition(delayLaunch, launching, HoldingFire, null, 0, 0, 1);
            var d_i = new Transition(delayLaunch, idle, StopFire, null, 0, 0, 1);
            var d_r = new Transition(delayLaunch, reload, CanReload, null, 0, 0);
            statemachine.AddTransitionFor(d_l);
            statemachine.AddTransitionFor(d_r);

            var l_i = new Transition(launching, idle, StopFire, null, 0, 0, 1);
            var r_i = new Transition(reload, idle, null, null, 0, 0, 1);
            statemachine.AddTransitionFor(l_i);
            statemachine.AddTransitionFor(r_i);
        }
        protected virtual bool HoldingFire()
        {
            return fireTrigger == null ? false : ammo.MagazineAmount > 0 && fireTrigger();
        }
        protected virtual bool StopFire()
        {
            return !HoldingFire();
        }
        protected virtual bool CanReload()
        {
            return reloadTrigger == null ? false : ammo.MagazineAmount != definitions.AmmoInMagazineAmount && reloadTrigger();
        }
        protected internal virtual IProjectile Launch()
        {
            var projectile = GetProjectile();
            var obj = projectile.Obj;
            WeaponsHelper.SynchronizeWorldTransform(obj.transform, _muzzleTrans);
            projectile.StartAction();
            ammo.MagazineAmount--;
            _launchedCallback?.Invoke(this);
            return projectile;
        }
        protected internal virtual void Reload()
        {
            var num = Mathf.Min(ammo.ReserveAmount, definitions.AmmoInMagazineAmount - ammo.MagazineAmount);
            ammo.MagazineAmount += num;
            ammo.ReserveAmount -= num;
            _reloadCallback?.Invoke(this);
        }
        public void FillReserve(int amount)
        {
            var result = ammo.ReserveAmount + amount;
            ammo.ReserveAmount = Mathf.Clamp(result, 0, definitions.AmmoTotalAmount - definitions.AmmoInMagazineAmount);
        }
        protected abstract IProjectile GetProjectile();
        protected abstract void ReleaseProjectile(IProjectile projectile);
    }
}
