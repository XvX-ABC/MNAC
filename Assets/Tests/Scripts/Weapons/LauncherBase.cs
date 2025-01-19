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
        protected ObjectPool<GameObject> projectilesPool;
        protected ILauncherDefines defines;
        [SerializeField]
        protected float launchingInterval;
        [SerializeField]
        protected float magazineRemainingCount;
        [SerializeField]
        protected float remainingCount;
        protected float lastLaunchingTIme;
        protected float lastReloadTime;
        protected Timeline reloadTimeline;
        public float RemainingCount { get => remainingCount; }
        public float MagazineRemainingCount { get => magazineRemainingCount; set => magazineRemainingCount = value; }
        protected virtual void Awake()
        {
            defines = GetComponent<ILauncherDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILauncherDefines));
        }
        protected virtual void OnEnable()
        {
            projectilesPool = new(
                () =>
                {
                    var origin = defines.ProjectileOrigin;
                    var obj = Instantiate(origin, this.transform);
                    obj.name = origin.name + "_" + projectilesPool.CountAll;
                    obj.transform.localPosition = defines.BorePosition;
                    obj.SetActive(false);


                    if (obj.TryGetComponent<IProjectile>(out var p))
                        p.HitAction += ReleaseBullet;
                    return obj;
                },
                obj =>
                {
            obj.transform.position = MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
                    obj.transform.SetParent(null);
                    obj.SetActive(true);
                    if (obj.TryGetComponent<IProjectile>(out var p))
                        p.Enabled = true;
                },
                obj =>
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(this.transform);
            obj.transform.localPosition = defines.MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
                    if (obj.TryGetComponent<IProjectile>(out var p))
                        p.Enabled = false;
                },
                obj =>
                {
                    if (obj.TryGetComponent<IProjectile>(out var p))
                        p.HitAction -= ReleaseBullet;
                }
                );
            reloadTimeline = new(defines.ReloadDuration);
            reloadTimeline.AddPointEvent(1, _ => Reload());
            //reloadTimeline = new(defines.ReloadDuration, false, new ReloadCompleted(this, 1));
        }
        protected virtual void OnDisable()
        {
            projectilesPool.Dispose();
        }

        protected virtual void Start()
        {
            if (defines.FiringRate > 0)
                launchingInterval = 1 / defines.FiringRate;
            else
                this.enabled = false;
            magazineRemainingCount = defines.ProjectilesTotalNumInMagazine;
            remainingCount = Mathf.Max(0, defines.ProjectilesTotalNum - defines.ProjectilesTotalNumInMagazine);
        }
        protected virtual void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
                Launch();
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
            if (Input.GetKeyDown(KeyCode.Space))
                Supply(10);
            if (reloadTimeline.IsRunning)
                reloadTimeline.OnUpdate(Time.deltaTime);
        }
        protected virtual void ReleaseBullet(IProjectile projectile, GameObject hitObj)
        {
            if (hitObj == this.gameObject)
                return;
            if (projectile is Component pobj)
                projectilesPool.Release(pobj.gameObject);
        }
        public void Supply(ushort num)
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled)
                return;
            if (reloadTimeline.IsRunning)
                return;
            remainingCount = Mathf.Min(defines.ProjectilesTotalNum - defines.ProjectilesTotalNumInMagazine, remainingCount + num);
        }
        public bool StartReload()
        {
            if (!enabled)
                return false;
            if (Time.time - lastReloadTime <= defines.ReloadDuration || remainingCount <= 0 || reloadTimeline.IsRunning)
                return false;
            reloadTimeline.Start();
            return true;
        }
        public bool EndReload()
        {
            if (!enabled || !reloadTimeline.IsRunning)
                return false;
            reloadTimeline.Stop();
            return true;
        }
        internal void Reload()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled)
                return;

            var num = Mathf.Min(remainingCount, defines.ProjectilesTotalNumInMagazine - magazineRemainingCount);
            magazineRemainingCount += num;
            remainingCount -= num;
            lastReloadTime = Time.time;
            return;
        }
        public void Launch()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled || reloadTimeline.IsRunning)
                return;
            var time = Time.time;
            if (time - lastLaunchingTIme <= launchingInterval || magazineRemainingCount <= 0)
                return;


            var obj = projectilesPool.Get();
            magazineRemainingCount--;
            lastLaunchingTIme = Time.time;
        }


    }
}