using System;
using System.Reflection;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using FoundationStone.UI.Tests.MVC;
using Tests.Utilities;
using Tests.Weapons.MissileLauncher;
using UnityEngine;
using UnityEngine.Pool;
using ActionsEnum = Tests.Weapons.Launcher.ILauncher.ActionsEnum;
namespace Tests.Weapons.Launcher
{
    [DisallowMultipleComponent]
    public class LauncherBase : MonoBehaviour, ILauncher
    {
        protected ObjectPool<GameObject> ammoPool;
        protected ILauncherDefinitions definitions;
        [SerializeField]
        protected ushort ammoInMagazineQuantity;
        [SerializeField]
        protected ushort ammoSpareQuantity;

        protected Action<ILauncher> initializationAction;
        protected float lastLaunchTime;


        protected ITimeline delayLaunchTimeline;
        protected ITimeline launchDurationTimeline;
        protected ITimeline reloadTimeline;



        protected LauncherActionsLock actionsLock;


        public string Name { get => this.name; }
        public ushort SpareCount { get => ammoSpareQuantity; }
        public ushort MagazineCount { get => ammoInMagazineQuantity; set => ammoInMagazineQuantity = value; }
        public ILauncherDefinitions Definitions { get => definitions; protected set => definitions = value; }
        public Vector3 MagazinePosition => this.transform.TransformPoint(definitions.MagazinePosition);
        public Vector3 MuzzlePosition => this.transform.TransformPoint(definitions.MuzzlePosition);
        public Action<ILauncher> InitializationAction { get => initializationAction; set => initializationAction = value; }
        public ITimeline DelayLaunchTimeline { get => delayLaunchTimeline; }
        public ITimeline LaunchDurationTimeline { get => launchDurationTimeline; }
        public ITimeline ReloadTimeline { get => reloadTimeline; }
        ILauncherActionsLock ILauncher.actionsLock { get => actionsLock; }

        WeaponType IWeapon.Type => WeaponType.Launcher;

        protected virtual void Awake()
        {
            definitions = GetComponent<ILauncherDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ILauncherDefinitions));
            actionsLock = new();

        }

        protected virtual void Start()
        {
            actionsLock = new();

            ammoPool = new(
             CreateAmmo,
             GetAmmo,
             ReleaseAmmo,
             DestroyAmmo
                );

            reloadTimeline = CreateReloadTimeline();
            delayLaunchTimeline = CreateDelayLaunchTimeline();
            launchDurationTimeline = CreateLaunchDurationTimeline();


            ammoInMagazineQuantity = definitions.AmmoInMagazineQuantity;
            ammoSpareQuantity = definitions.AmmoSpareQuantity;
            initializationAction?.Invoke(this);
            InitializationAction = null;
        }
        protected virtual void Update()
        {
            if (delayLaunchTimeline != null && delayLaunchTimeline.IsRunning)
                delayLaunchTimeline.OnUpdate(Time.deltaTime);
            if (launchDurationTimeline != null && launchDurationTimeline.IsRunning)
                launchDurationTimeline.OnUpdate(Time.deltaTime);
            if (reloadTimeline != null && reloadTimeline.IsRunning)
                reloadTimeline.OnUpdate(Time.deltaTime);
        }
        protected void OnDestroy()
        {
            ammoPool.Dispose();
        }
        protected virtual GameObject CreateAmmo()
        {
            var origin = definitions.AmmoOrigin;
            var obj = Instantiate(origin, transform);
            obj.name = origin.name + "_" + ammoPool.CountAll;

            obj.SetActive(false);


            if (obj.TryGetComponent<IProjectile>(out var p))
                p.HitAction += ReleaseAmmo;
            return obj;
        }
        protected virtual void GetAmmo(GameObject obj)
        {
            obj.transform.position = MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.SetParent(null);
            obj.SetActive(true);
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.Enabled = true;
        }
        protected virtual void ReleaseAmmo(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = definitions.MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.Enabled = false;
        }
        protected virtual void ReleaseAmmo(IProjectile ammo, GameObject hitObj)
        {
            if (hitObj == gameObject)
                return;
            if (ammo is Component pobj)
                ammoPool.Release(pobj.gameObject);
        }
        protected virtual void DestroyAmmo(GameObject obj)
        {
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.HitAction -= ReleaseAmmo;
        }

        protected virtual ITimeline CreateReloadTimeline()
        {
            var timeline = new Timeline(definitions.ReloadDurationTime);
            timeline.AddPointEvent(0, _ => actionsLock.LockStartReload());
            timeline.AddPointEvent(1, _ =>
            {
                DoReload();
                actionsLock.UnlockAll();
            });
            return timeline;
        }
        protected virtual ITimeline CreateDelayLaunchTimeline()
        {
            var timeline = new RandomLengthTimeline(definitions.LaunchDelayRange);
            timeline.AddPointEvent(0, _ => actionsLock.LockStartLaunch());
            timeline.AddPointEvent(1, _ =>
            {
                Launch();
                actionsLock.UnlockAll();
                launchDurationTimeline.Start();
            });
            return timeline;
        }
        protected virtual ITimeline CreateLaunchDurationTimeline()
        {
            var timeline = new Timeline(definitions.LaunchDurationTime);
            timeline.AddPointEvent(0, _ => actionsLock.LockAll());
            timeline.AddPointEvent(1, _ => actionsLock.UnlockAll());
            return timeline;
        }


        public int Fill(int num)
        {
            if (!enabled || actionsLock.IsLocked(ActionsEnum.Supply) || num == 0)
                return 0;
            var suppNum = 0;
            if (num > 0)
                suppNum = Mathf.Min(definitions.AmmoSpareQuantity - ammoSpareQuantity, num);
            else
                suppNum = Mathf.Min(-(definitions.AmmoSpareQuantity - ammoSpareQuantity), num);
            ammoSpareQuantity += (ushort)suppNum;
            return suppNum;
        }
        public virtual bool StartReload()
        {
            if (!enabled)
                return false;
            if (actionsLock.StartReloadLocked()
                || ammoInMagazineQuantity == definitions.AmmoInMagazineQuantity
                || ammoSpareQuantity <= 0)
                return false;
            reloadTimeline.Start();
            return true;
        }
        public virtual bool EndReload()
        {
            if (!enabled || actionsLock.EndReloadLocked())
                return false;
            reloadTimeline.Stop();
            return true;
        }
        internal virtual void DoReload()
        {
            if (!enabled)
                return;

            var num = (ushort)Mathf.Min(ammoSpareQuantity, definitions.AmmoInMagazineQuantity - ammoInMagazineQuantity);
            ammoInMagazineQuantity += num;
            ammoSpareQuantity -= num;
            return;
        }

        public virtual bool StartLaunch()
        {
            if (!enabled || actionsLock.StartLaunchLocked())
                return false;
            if (ammoInMagazineQuantity <= 0)
                return false;
            delayLaunchTimeline.Start();
            return true;
        }
        public virtual bool EndLaunch()
        {
            if (!enabled || actionsLock.EndLaunchLocked())
                return false;
            else if (delayLaunchTimeline.IsRunning)
            {
                delayLaunchTimeline.Stop();
                return true;
            }
            else if (launchDurationTimeline.IsRunning)
                return false;
            return true;
        }
        internal virtual void Launch()
        {
            var obj = ammoPool.Get();
            ammoInMagazineQuantity--;
        }

#if UNITY_EDITOR
        ILauncherDefinitionsEditor _definitionsEditor;
        protected virtual void OnDrawGizmosSelected()
        {
            if (definitions == null)
            {
                definitions = GetComponent<ILauncherDefinitions>();
                _definitionsEditor = definitions as ILauncherDefinitionsEditor;
                _definitionsEditor?.Load();
            }
            if (definitions != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(MuzzlePosition, 0.1f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(MagazinePosition, 0.1f);
            }
        }
#endif

    }
}