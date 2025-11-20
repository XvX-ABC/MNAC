using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Utilities.Timeline;
using Tests.Weapons.MachineGuns;
using UnityEngine;
using UnityEngine.Pool;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Utilities.Timeline.Events.Range;
using ActionsEnum = Tests.Weapons.Launcher.ILauncher_Obsolete.ActionsEnum;
namespace Tests.Weapons.Launcher
{
    [DisallowMultipleComponent]
    public class LauncherBase : MonoBehaviour, ILauncher_Obsolete
    {
        [Serializable]
        protected internal class EffectorSupporter : ILauncherEffector
        {
            ILauncherEffector[] _effectors;
            ILauncher_Obsolete _owner;
            public EffectorSupporter(ILauncherEffector[] effectors)
            {
                _effectors = effectors;
            }

            public ILauncher_Obsolete Owner => _effectors[0].Owner;


            public void Initialize(ILauncher_Obsolete owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                foreach (var e in _effectors)
                    e.Initialize(owner);
            }
            public void Dispose()
            {
                foreach (var e in _effectors)
                {
                    e.Dispose();
                }
            }

        }
        [SerializeField]
        GameObject[] _effectorObjs;
        protected EffectorSupporter effectorSupporter;

        protected ObjectPool<GameObject> ammoPool;
        protected ILauncherDefinitions definitions;
        [SerializeField]
        protected ushort ammoInMagazineQuantity;
        [SerializeField]
        protected ushort ammoReservesQuantity;

        protected Action<ILauncher_Obsolete> initializationAction;
        protected float lastLaunchTime;


        protected ITimeline delayLaunchTimeline;
        protected ITimeline launchDurationTimeline;
        protected ITimeline reloadTimeline;


        internal WeaponLoad load;


        protected LauncherActionsLock actionsLock;
        protected Action<ILauncher_Obsolete> launchAction;
        protected Action<ILauncher_Obsolete> reloadAction;

        public string Name { get => this.name; }
        public ushort ReservesAmmoCount { get => ammoReservesQuantity; }
        public ushort MagazineAmmoCount { get => ammoInMagazineQuantity; set => ammoInMagazineQuantity = value; }
        public ILauncherDefinitions Definitions { get => definitions; protected set => definitions = value; }
        public Vector3 MagazinePosition => this.transform.TransformPoint(definitions.MagazinePosition);
        public Vector3 MuzzlePosition => this.transform.TransformPoint(definitions.MuzzlePosition);
        public Action<ILauncher_Obsolete> InitializationAction { get => initializationAction; set => initializationAction = value; }
        public ITimeline DelayLaunchTimeline { get => delayLaunchTimeline; }
        public ITimeline LaunchDurationTimeline { get => launchDurationTimeline; }
        public ITimeline ReloadTimeline { get => reloadTimeline; }
        ILauncherActionsLock ILauncher_Obsolete.actionsLock { get => actionsLock; }

        WeaponType IWeapon_Obsolete.Type => WeaponType.Launcher;

        public Action<ILauncher_Obsolete> LaunchAction { get => launchAction; set => launchAction = value; }
        public Action<ILauncher_Obsolete> ReloadAction { get => reloadAction; set => reloadAction = value; }

        public GameObject Obj => this.gameObject;

        protected virtual void Awake()
        {
            definitions = GetComponent<ILauncherDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ILauncherDefinitions));
            actionsLock = new();

            reloadTimeline = CreateReloadTimeline();
            delayLaunchTimeline = CreateDelayLaunchTimeline();
            launchDurationTimeline = CreateLaunchDurationTimeline();


            ammoInMagazineQuantity = definitions.AmmoInMagazineQuantity;
            ammoReservesQuantity = definitions.AmmoReservesQuantity;


            InitializeEffectors();
            effectorSupporter?.Initialize(this);

            load = GetComponent<WeaponLoad>();
        }
        protected virtual void InitializeEffectors()
        {
            var effectors = new List<ILauncherEffector>();
            foreach (var obj in _effectorObjs)
            {
                if (obj == null)
                {
                    Debug.LogWarning(new NullReferenceException(nameof(obj)));
                    continue;
                }
                var e = obj.GetComponents<ILauncherEffector>();
                if (e == null)
                {
                    Debug.LogWarning($"The obj '{obj.name}' doesn't have a effector.");
                    continue;
                }
                effectors.AddRange(e);
            }
            if (effectors.Count > 0)
                effectorSupporter = new(effectors.ToArray());
        }
        protected virtual void Start()
        {


            ammoPool = new(
             CreateAmmo,
             GetAmmo,
             ReleaseAmmo,
             DestroyAmmo
                );


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
            {
                reloadTimeline.OnUpdate(Time.deltaTime);
            }
        }
        protected void OnDestroy()
        {
            ammoPool.Dispose();
            effectorSupporter?.Dispose();
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
            timeline.StartAction += _ => actionsLock.LockStartReload();
            timeline.EndAction += _ =>
            {
                Reload();
                actionsLock.UnlockAll();
            };
            //timeline.AddPointEvent(0, _ => actionsLock.LockStartReload());
            //timeline.AddPointEvent(1, _ =>
            //{
            //    Reload();
            //    actionsLock.UnlockAll();
            //});

            return timeline;
        }
        protected virtual ITimeline CreateDelayLaunchTimeline()
        {
            var timeline = new RandomLengthTimeline(definitions.LaunchDelayRange);
            timeline.AddPointEvent(0, _ => actionsLock.LockStartLaunch());
            timeline.AddPointEvent(1, _ =>
            {

                actionsLock.UnlockAll();
                launchDurationTimeline.Restart();
            });
            return timeline;
        }
        protected virtual ITimeline CreateLaunchDurationTimeline()
        {
            var timeline = new Timeline(definitions.LaunchDurationTime);
            timeline.AddPointEvent(0, _ =>
            {
                Launch();
                actionsLock.LockAll();
            });
            timeline.AddPointEvent(1, _ => actionsLock.UnlockAll());
            return timeline;
        }


        public int Fill(int num)
        {
            if (!enabled || actionsLock.IsLocked(ActionsEnum.Supply) || num == 0)
                return 0;
            var suppNum = 0;
            if (num > 0)
                suppNum = Mathf.Min(definitions.AmmoReservesQuantity - ammoReservesQuantity, num);
            else
                suppNum = Mathf.Min(-(definitions.AmmoReservesQuantity - ammoReservesQuantity), num);
            ammoReservesQuantity += (ushort)suppNum;
            return suppNum;
        }
        public virtual bool StartReload()
        {
            if (!enabled)
                return false;
            if (actionsLock.StartReloadLocked()
                || ammoInMagazineQuantity == definitions.AmmoInMagazineQuantity
                || ammoReservesQuantity <= 0)
                return false;
            reloadTimeline.Restart();
            return true;
        }
        public virtual bool EndReload()
        {
            if (!enabled || actionsLock.EndReloadLocked())
                return false;
            //reloadTimeline.Pause();
            reloadTimeline.End();
            return true;
        }
        internal virtual void Reload()
        {
            if (!enabled)
                return;
            var num = (ushort)Mathf.Min(ammoReservesQuantity, definitions.AmmoInMagazineQuantity - ammoInMagazineQuantity);
            ammoInMagazineQuantity += num;
            ammoReservesQuantity -= num;
            reloadAction?.Invoke(this);
            return;
        }

        public virtual bool StartLaunch()
        {
            if (!enabled || actionsLock.StartLaunchLocked())
                return false;
            if (ammoInMagazineQuantity <= 0)
                return false;
            delayLaunchTimeline.Restart();
            return true;
        }
        public virtual bool EndLaunch()
        {
            if (!enabled || actionsLock.EndLaunchLocked())
                return false;
            else if (delayLaunchTimeline.IsRunning)
            {
                delayLaunchTimeline.Pause();
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
            launchAction?.Invoke(this);
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

        public virtual void WhenMounted(GameObject mountPoint)
        {
            load?.WhenMounted(mountPoint);
        }

        public virtual void WhenUnmounted(GameObject mountPoint)
        {
            load?.WhenUnmounted(mountPoint);
        }
#endif

    }
}