using System;
using System.Reflection;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Tests.Weapons
{

    public class LauncherBase : MonoBehaviour, ILauncher
    {
        [Obsolete]
        protected class ReloadCompleted : PointEvent
        {
            LauncherBase _gun;
            public ReloadCompleted(LauncherBase gun, float triggeredProportion) : base(triggeredProportion)
            {
                _gun = gun;
            }

            public override void Execute(TimelineContext context)
            {
                _gun.Reload();
            }
        }
        protected ObjectPool<GameObject> ammoPool;
        protected ILauncherDefines defines;
        [SerializeField]
        protected float launchingInterval;
        [SerializeField]
        protected float LaunchingDelay;
        [SerializeField]
        protected ushort ammoQuantityInMagazine;
        [SerializeField]
        protected ushort ammoSpareQuantity;

        private Action<ILauncher> initializationAction;
        protected float lastLaunchTime;
        protected float lastReloadTime;
        protected Timeline reloadTimeline;
        protected Timeline launchDelayTimeline;
        public ushort SpareCount { get => ammoSpareQuantity; }
        public ushort MagazineCount { get => ammoQuantityInMagazine; set => ammoQuantityInMagazine = value; }
        public ILauncherDefines Defines { get => defines; }
        public Vector3 MagazinePosition { get => this.transform.position + this.transform.rotation * defines.MagazinePosition; }
        public Action<ILauncher> InitializationAction { get => initializationAction; set => initializationAction = value; }

        protected virtual void Awake()
        {
            defines = GetComponent<ILauncherDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherDefines));
            actionsLock = new();
        }
        protected virtual void OnEnable()
        {



            //reloadTimeline = new(defines.ReloadDuration, false, new ReloadCompleted(this, 1));
        }
        protected virtual void OnDisable()
        {
            ammoPool.Dispose();
        }

        protected virtual void Start()
        {
            ammoPool = new(
             CreateAmmo,
             GetAmmo,
             ReleaseAmmo,
             DestroyAmmo
                );

            reloadTimeline = CreateReloadTimeline();
            launchDelayTimeline = CreateLaunchDelayTimeline();

            if (defines.FireRate > 0 && defines.FireDelay > 0)
                launchingInterval = 1 / defines.FireRate;
            else
                this.enabled = false;
            ammoQuantityInMagazine = defines.AmmoQuantityInMagazine;
            ammoSpareQuantity = defines.AmmoSpareQuantity;
            initializationAction?.Invoke(this);
        }
        protected virtual void Update()
        {
            //if (Input.GetKey(KeyCode.Mouse0))
            //    Launch();
            //if (Input.GetKeyDown(KeyCode.R))
            //    Reload();
            //if (Input.GetKeyDown(KeyCode.Space))
            //    Supply(10);
            if (reloadTimeline.IsRunning)
                reloadTimeline.OnUpdate(Time.deltaTime);
        }
        protected virtual GameObject CreateAmmo()
        {
            var origin = defines.AmmoOrigin;
            var obj = Instantiate(origin, this.transform);
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
            obj.transform.SetParent(this.transform);
            obj.transform.localPosition = defines.MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.Enabled = false;
        }
        protected virtual void DestroyAmmo(GameObject obj)
        {
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.HitAction -= ReleaseAmmo;
        }

        protected virtual Timeline CreateReloadTimeline()
        {
            var timeline = new Timeline(defines.ReloadDuration);
            timeline.AddPointEvent(1, _ => Reload());
            return timeline;
        }

        protected virtual Timeline CreateLaunchDelayTimeline()
        {
            var timeline = new Timeline(defines.FireDelay);
            return timeline;
        }
        protected virtual void ReleaseAmmo(IProjectile ammo, GameObject hitObj)
        {
            if (hitObj == this.gameObject)
                return;
            if (ammo is Component pobj)
                ammoPool.Release(pobj.gameObject);
        }
        public int Supply(int num)
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled || reloadTimeline.IsRunning || ammoSpareQuantity + num < 0)
                return 0;
            var suppNum = Mathf.Min(Mathf.Max(0, defines.AmmoSpareQuantity) - ammoSpareQuantity, num);
            ammoSpareQuantity += (ushort)suppNum;
            //remainingCount =(ushort) Mathf.Min(defines.AmmoTotalNum - defines.AmmoTotalNumInMagazine, remainingCount + num);
            return suppNum;
        }
        public virtual bool StartReload()
        {
            if (!enabled)
                return false;
            if (Time.time - lastReloadTime <= defines.ReloadDuration || ammoSpareQuantity <= 0 || reloadTimeline.IsRunning)
                return false;
            reloadTimeline.Start();
            return true;
        }
        public virtual bool EndReload()
        {
            if (!enabled || !reloadTimeline.IsRunning)
                return false;
            reloadTimeline.Stop();
            return true;
        }
        internal virtual void Reload()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled)
                return;

            var num = (ushort)Mathf.Min(ammoSpareQuantity, defines.AmmoQuantityInMagazine - ammoQuantityInMagazine);
            ammoQuantityInMagazine += num;
            ammoSpareQuantity -= num;
            lastReloadTime = Time.time;
            return;
        }
        public virtual void Launch()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled || reloadTimeline.IsRunning)
                return;
            var time = Time.time;
            if (time - lastLaunchTime <= launchingInterval || ammoQuantityInMagazine <= 0)
                return;


            var obj = ammoPool.Get();
            ammoQuantityInMagazine--;
            lastLaunchTime = Time.time;
        }


    }
}