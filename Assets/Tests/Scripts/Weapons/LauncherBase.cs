using System;
using System.Reflection;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using FoundationStone.UI.Tests.MVC;
using UnityEngine;
using UnityEngine.Pool;
using ActionsEnum = Tests.Weapons.ILauncher.ActionsEnum;
namespace Tests.Weapons
{
    [DisallowMultipleComponent]
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
        protected ILauncherDefinitions definition;
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
        //protected float lastReloadTime;


        private ITimeline reloadTimeline;



        protected LauncherActionsLock actionsLock;


        public ushort SpareCount { get => ammoSpareQuantity; }
        public ushort MagazineCount { get => ammoQuantityInMagazine; set => ammoQuantityInMagazine = value; }
        public ILauncherDefinitions Definition { get => definition; protected set => definition = value; }
        public Vector3 MagazinePosition { get => this.transform.position + this.transform.rotation * definition.MagazinePosition; }
        public Action<ILauncher> InitializationAction { get => initializationAction; set => initializationAction = value; }
        public ITimeline ReloadTimeline { get => reloadTimeline; }
        ILauncherActionsLock ILauncher.actionsLock { get => actionsLock; }

        protected virtual void Awake()
        {
            definition = GetComponent<ILauncherDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherDefinitions));
            actionsLock = new();

        }
        protected virtual void OnEnable()
        {

            //reloadTimeline = new(definition.ReloadDuration, false, new ReloadCompleted(this, 1));
        }
        protected virtual void OnDisable()
        {
            ammoPool.Dispose();
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

            if (definition.LaunchRate > 0)
                launchingInterval = 1 / definition.LaunchRate;
            else
                this.enabled = false;
            ammoQuantityInMagazine = definition.AmmoQuantityInMagazine;
            ammoSpareQuantity = definition.AmmoSpareQuantity;
            initializationAction?.Invoke(this);
            InitializationAction = null;
        }
        protected virtual void Update()
        {
            if (reloadTimeline.IsRunning)
                reloadTimeline.OnUpdate(Time.deltaTime);
        }
        protected virtual GameObject CreateAmmo()
        {
            var origin = definition.AmmoOrigin;
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
            obj.transform.localPosition = definition.MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.Enabled = false;
        }
        protected virtual void DestroyAmmo(GameObject obj)
        {
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.HitAction -= ReleaseAmmo;
        }

        protected virtual ITimeline CreateReloadTimeline()
        {
            var timeline = new Timeline(definition.ReloadDuration);
            timeline.AddPointEvent(0, _ => actionsLock.LockAll());
            timeline.AddPointEvent(1, _ =>
            {
                Reload();
                actionsLock.UnlockAll();
            });
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
            if (!enabled || actionsLock.IsLocked(ActionsEnum.Supply) || num == 0)
                return 0;
            var suppNum = 0;
            if (num > 0)
                suppNum = Mathf.Min(definition.AmmoSpareQuantity - ammoSpareQuantity, num);
            else
                suppNum = Mathf.Min(-(definition.AmmoSpareQuantity - ammoSpareQuantity), num);
            ammoSpareQuantity += (ushort)suppNum;
            //remainingCount =(ushort) Mathf.Min(definition.AmmoTotalNum - definition.AmmoTotalNumInMagazine, remainingCount + num);
            return suppNum;
        }
        public virtual bool StartReload()
        {
            if (!enabled)
                return false;
            if (actionsLock.StartReloadLocked()
                || ammoQuantityInMagazine == definition.AmmoQuantityInMagazine
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
        internal virtual void Reload()
        {
            if (!enabled)
                return;

            var num = (ushort)Mathf.Min(ammoSpareQuantity, definition.AmmoQuantityInMagazine - ammoQuantityInMagazine);
            ammoQuantityInMagazine += num;
            ammoSpareQuantity -= num;
            //lastReloadTime = Time.time;
            return;
        }
        public virtual bool Launch()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled || actionsLock.LaunchLocked())
                return false;
            var time = Time.time;
            if (time - lastLaunchTime <= launchingInterval || ammoQuantityInMagazine <= 0)
                return false;


            var obj = ammoPool.Get();
            ammoQuantityInMagazine--;
            lastLaunchTime = Time.time;

            return true;
        }
        [RequestMapping("{c_url}/ReloadTimeline/UpdateEvent/Register", RequestMethod.GET)]
        public void RegisterReloadTimelineUpdateEvent(IRequest<(bool, Action<float>)> request, IResponse response)
        {
            var register = request.Data.Item1;
            if (register)
                reloadTimeline.UpdateAction += request.Data.Item2;
            else
                reloadTimeline.UpdateAction -= request.Data.Item2;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
    }
}