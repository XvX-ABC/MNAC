using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using System.Linq;
using Tests.Utilities;
using Tests.Weapons.Launcher;
using Tests.Weapons.MissileLauncher;
using UnityEngine;
using UInput = UnityEngine.Input;
namespace Tests.Weapons.MultiMissileLauncher
{

    public class MultiMissileLauncher : MonoBehaviour, IMissileLauncher
    {
        public static IMissileLauncher[] GetSubLaunchers(GameObject obj)
        {
            var m = obj.GetComponent<IMissileLauncher>();
            var list = obj.GetComponentsInChildren<IMissileLauncher>().ToList();
            if (m != null && list.Contains(m))
                list.Remove(m);
            return list.ToArray();
        }

        ushort _ammoSpareQuantity;
        ushort _ammoInMagazineQuantity;
        Action<ILauncher> _initializationAction;
        Action<IMissileLauncher, ITarget> _targetChangeAction;
        ITarget _target;
        ILauncherActionsLock _actionsLock;
        ITimeline _reloadTimeline;
        ITimeline _delayLaunchTimeline;
        ITimeline _launchDurationTimeline;
        protected IMissileLauncherDefinitions definitions;
        internal IMissileLauncher[] subLaunchers;
        public ushort SpareCount { get => _ammoSpareQuantity; }
        public ushort MagazineCount { get => _ammoInMagazineQuantity; }
        public ILauncherDefinitions Definitions { get => definitions; }
        IMissileLauncherDefinitions IMissileLauncher.Definitions => definitions;
        public Action<ILauncher> InitializationAction { get => _initializationAction; set => _initializationAction = value; }
        public ITarget Target
        {
            get => _target;
            set
            {
                _targetChangeAction?.Invoke(this, value);
                _target = value;
                foreach (var l in subLaunchers)
                    l.Target = value;
            }
        }

        public ITimeline LaunchDurationTimeline { get => _launchDurationTimeline; }
        public ITimeline ReloadTimeline { get => _reloadTimeline; }
        public ITimeline DelayLaunchTimeline { get => _delayLaunchTimeline; }
        public Action<IMissileLauncher, ITarget> TargetChangeAction
        {
            get
            {
                return _targetChangeAction;
            }
            set
            {
                _targetChangeAction = value;
            }
        }
        ILauncherActionsLock ILauncher.actionsLock => _actionsLock;


        string IWeapon.Name => this.gameObject.name;

        WeaponType IWeapon.Type => WeaponType.Launcher;

        public IMissileLauncher this[int index]
        {
            get
            {
                if (index < 0 || index >= subLaunchers.Length)
                    throw new IndexOutOfRangeException("index: " + index);
                return subLaunchers[index];
            }
            set
            {
                if (index < 0 || index >= subLaunchers.Length)
                    throw new IndexOutOfRangeException("index: " + index);
                subLaunchers[index] = value;
            }
        }

        void Awake()
        {
            definitions = GetComponent<IMissileLauncherDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ILauncherDefinitions));
            LoadSubLaunchers();

            foreach (var l in subLaunchers)
                InitializeSubLauncher(l);





        }


        protected void LoadSubLaunchers()
        {
            //var list = GetComponentsInChildren<IMissileLauncher>().ToList();
            //if (list.Contains(this))
            //    list.Remove(this);
            //subLaunchers = list.ToArray();
            subLaunchers = GetSubLaunchers(gameObject);
        }
        protected void InitializeSubLauncher(IMissileLauncher l)
        {
            l.InitializationAction += launcher =>
            {
                launcher.Fill(-launcher.Definitions.AmmoSpareQuantity);
                if (launcher is not IMissileLauncher mlauncher)
                    throw new Exception($"The sublaunchers of the type '{GetType().Name}' must to implement the interface '{typeof(IMissileLauncher).Name}'");


                var definition = mlauncher.Definitions;
                if (definition.AmmoSpareQuantity == 0)
                {
                    if ((object)mlauncher is GameObject obj)
                        throw new Exception($"The parameter 'AmmoSpareQuantity' of the subluncher '{obj.name}' can't less than or equals to zero.");
                    else
                        throw new Exception($"The parameter 'AmmoSpareQuantity' of the subluncher can't less than or equals to zero.");
                }

                var reloadTimeline = mlauncher.ReloadTimeline;
                reloadTimeline.EndAction += context =>
                {
                    EndReloadForSubLauncher(mlauncher);
                };

            };


        }
        protected void Start()
        {
            _ammoSpareQuantity = (ushort)Mathf.Max(0, definitions.AmmoSpareQuantity - subLaunchers.Length);

            var quantity = subLaunchers.Length;
            _ammoInMagazineQuantity = (ushort)quantity;

            //var reloadTimelines = new ITimeline[quantity];
            //var delayLaunchTimelines = new ITimeline[quantity];
            //for (int i = 0; i < quantity; i++)
            //{
            //    var l = subLaunchers[i];
            //    reloadTimelines[i] = l.ReloadTimeline;
            //    delayLaunchTimelines[i] = l.DelayLaunchTimeline;
            //}


            //_reloadTimeline = new TimelinesGroup(reloadTimelines);
            //_delayLaunchTimeline = new TimelinesGroup(delayLaunchTimelines);
            _reloadTimeline = CreateReloadTimeline();
            _delayLaunchTimeline = CreateDelayLaunchTimeline();
            _launchDurationTimeline = CreateLaunchDurationTimeline();
            var actionsLocks = subLaunchers.Select(launcher => launcher.actionsLock).ToArray();
            //_actionsLock = new LauncherActionsLockGroup(actionsLocks);
            _actionsLock = new LauncherActionsLock();

            _initializationAction?.Invoke(this);
        }
        protected void Update()
        {
            if (_delayLaunchTimeline.IsRunning)
                _delayLaunchTimeline.OnUpdate(Time.deltaTime);
            if (_launchDurationTimeline.IsRunning)
                _launchDurationTimeline.OnUpdate(Time.deltaTime);
            if (_reloadTimeline.IsRunning)
                _reloadTimeline.OnUpdate(Time.deltaTime);
            if (UInput.GetKey(KeyCode.Mouse0))
            {
                Debug.Log("StartLaunch");
                StartLaunch();
            }
            if (UInput.GetKeyDown(KeyCode.R))
            {
                var result = StartReload();
                Debug.Log("Start reload result: " + result);
            }
            if (UInput.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Supply");
                Fill(10);
            }

            if (UInput.GetKeyDown(KeyCode.S))
            {
                if (Target == null)
                    Target = GetComponent<ITarget>() ?? throw new ComponentCantFindException(gameObject, typeof(ITarget));
                else
                    Target = null;
            }

        }
        protected virtual ITimeline CreateDelayLaunchTimeline()
        {
            //return new TimelinesGroup(l => l.DelayLaunchTimeline, subLaunchers);
            var timeline = new Timeline(definitions.LaunchDelayRange.y);
            timeline.AddPointEvent(0, _ => _actionsLock.LockAll());
            timeline.AddPointEvent(1, _ => { _actionsLock.UnlockAll(); _launchDurationTimeline.Restart(); });
            return timeline;
        }
        protected virtual ITimeline CreateLaunchDurationTimeline()
        {
            //return new TimelinesGroup(l => l.LaunchDurationTimeline, subLaunchers);
            var timeline = new Timeline(definitions.LaunchDurationTime);
            timeline.AddPointEvent(0, _ => _actionsLock.LockAll());
            timeline.AddPointEvent(1, _ => _actionsLock.UnlockAll());
            return timeline;
        }
        protected virtual ITimeline CreateReloadTimeline()
        {
            //return new TimelinesGroup(l => l.ReloadTimeline, subLaunchers);
            var timeline = new Timeline(definitions.ReloadDurationTime);
            timeline.AddPointEvent(0, _ => _actionsLock.LockAll());
            timeline.AddPointEvent(1, _ => _actionsLock.UnlockAll());
            return timeline;
        }
        public bool StartLaunch()
        {
            if (!enabled || _actionsLock.StartLaunchLocked() || _ammoInMagazineQuantity <= 0)
                return false;

            var v = _ammoInMagazineQuantity;
            foreach (var l in subLaunchers)
            {
                if (_ammoInMagazineQuantity <= 0)
                    break;
                if (l.StartLaunch())
                    _ammoInMagazineQuantity--;
            }
            if (_ammoInMagazineQuantity != v)
                _delayLaunchTimeline.Restart();
            return true;
        }
        public bool EndLaunch()
        {
            if (!enabled || _actionsLock.EndReloadLocked())
                return false;

            foreach (var l in subLaunchers)
            {
                var al = l.actionsLock;
                if (al.StartLaunchLocked())
                    if (l.EndLaunch())
                        _ammoInMagazineQuantity++;
                    else
                        return false;
            }
            _delayLaunchTimeline.Pause();
            return true;
        }

        public virtual bool StartReload()
        {
            if (!enabled || _actionsLock.StartReloadLocked())
                return false;

            var v = _ammoSpareQuantity;
            foreach (var l in subLaunchers)
            {
                if (_ammoSpareQuantity <= 0)
                    return false;

                l.Fill(1);
                _ammoSpareQuantity--;
                if (!l.StartReload())
                {
                    l.Fill(-1);
                    _ammoSpareQuantity++;
                    return false;
                }
            }
            if (_ammoSpareQuantity != v)
                _reloadTimeline.Restart();
            return true;
        }
        internal bool EndReloadForSubLauncher(IMissileLauncher launcher)
        {
            if (launcher.EndReload())
            {
                _ammoInMagazineQuantity++;
                return true;
            }
            launcher.Fill(-1);
            _ammoSpareQuantity++;
            return false;
        }
        public virtual bool EndReload()
        {
            if (!enabled || _actionsLock.EndReloadLocked())
                return false;
            foreach (var l in subLaunchers)
            {
                if (!EndReloadForSubLauncher(l))
                    return false;
            }
            _reloadTimeline.Pause();
            return true;
        }

        public int Fill(int num)
        {
            if (_ammoSpareQuantity + num < 0 || _actionsLock.SupplyLocked())
                return 0;
            //var suppNum = Mathf.Min(definitions.AmmoTotalQuantity - subLaunchers.Length - _ammoSpareQuantity, num);
            var suppNum = Mathf.Min(definitions.AmmoSpareQuantity - _ammoSpareQuantity, num);
            _ammoSpareQuantity += (ushort)suppNum;
            return suppNum;
        }

        void OnGUI()
        {
            var style = new GUIStyle();
            style.fontSize = 32;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, 100, 100), _ammoSpareQuantity.ToString(), style);
            GUI.Label(new Rect(0, 100, 100, 100), _ammoInMagazineQuantity.ToString(), style);
            if (Target == null)
                GUI.Label(new Rect(0, 200, 100, 100), "NULL", style);
            else
                GUI.Label(new Rect(0, 200, 100, 100), Target.Obj.name, style);
        }
    }
}
