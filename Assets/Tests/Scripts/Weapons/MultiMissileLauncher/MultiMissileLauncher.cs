using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using System;
using System.Linq;
using Tests;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    [Serializable]
    public struct PrepareLaunch
    {
        public float CoverOpenDuration;
    }
    [Serializable]
    public struct Reload
    {
        public Vector2 CoverCloseProportion;
        public Vector2 MagazineEmptyProportion;
        public Vector2 MagazineFullProportion;
        public float CoverCloseTriggerProportion;
        public float MagazineEmptyTriggerProportion;
        public float MagazineFullTriggerProportion;
    }
    [Serializable]
    public struct Cover
    {
        public float OpenOrCloseDuration;
    }
    [Serializable]
    public struct MagazineModule
    {
        public float EmptyOrFullDuration;
    }

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

        internal IMissileLauncher[] subLaunchers;
        ushort _ammoSpareQuantity;
        ushort _ammoQuantityInMagazine;
        IMissileLauncherDefinitions _definitions;
        Action<ILauncher> _initializationAction;
        Action<IMissileLauncher, ITarget> _targetChangeAction;
        ITarget _target;
        ILauncherActionsLock _actionsLock;
        ITimeline _reloadTimeline;
        ITimeline _delayLaunchTimeline;
        ITimeline _launchDurationTimeline;
        public ushort SpareCount { get => _ammoSpareQuantity; }
        public ushort MagazineCount { get => _ammoQuantityInMagazine; }
        public ILauncherDefinitions Definitions { get => _definitions; }
        IMissileLauncherDefinitions IMissileLauncher.Definitions => _definitions;
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
            _definitions = GetComponent<IMissileLauncherDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherDefinitions));
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
            subLaunchers = GetSubLaunchers(this.gameObject);
        }
        protected void InitializeSubLauncher(IMissileLauncher l)
        {
            l.InitializationAction += launcher =>
            {
                launcher.Supply(-(launcher.Definitions.AmmoSpareQuantity));
                if (launcher is not IMissileLauncher mlauncher)
                    throw new Exception($"The sublaunchers of the type '{this.GetType().Name}' must to implement the interface '{typeof(IMissileLauncher).Name}'");


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
            _ammoSpareQuantity = (ushort)(_definitions.AmmoTotalQuantity - subLaunchers.Length);


            var quantity = subLaunchers.Length;
            _ammoQuantityInMagazine = (ushort)quantity;

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
            _reloadTimeline = new TimelinesGroup(l => ((IMissileLauncher)l).ReloadTimeline, subLaunchers);
            _delayLaunchTimeline = new TimelinesGroup(l => ((IMissileLauncher)l).DelayLaunchTimeline, subLaunchers);
            _launchDurationTimeline = new TimelinesGroup(l => l.LaunchDurationTimeline, subLaunchers);
            var actionsLocks = subLaunchers.Select(launcher => launcher.actionsLock).ToArray();
            _actionsLock = new LauncherActionsLockGroup(actionsLocks);

            _initializationAction?.Invoke(this);
        }
        protected void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
                StartLaunch();
            if (Input.GetKeyDown(KeyCode.R))
            {
                var result = StartReload();
                Debug.Log("Start reload result: " + result);
            }
            if (Input.GetKeyDown(KeyCode.Space))
                Supply(10);

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Target == null)
                    Target = GetComponent<ITarget>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ITarget));
                else
                    Target = null;
            }

        }
        public bool StartLaunch()
        {
            if (!enabled || _actionsLock.StartLaunchLocked() || _ammoQuantityInMagazine <= 0)
                return false;


            foreach (var l in subLaunchers)
            {
                if (_ammoQuantityInMagazine <= 0)
                    break;
                if (l.StartLaunch())
                    _ammoQuantityInMagazine--;
            }
            return true;
        }
        public bool EndLaunch()
        {
            if (!enabled || _actionsLock.EndReloadLocked())
                return false;

            foreach (var l in subLaunchers)
            {
                if(_ammo)
            }
        }

        public bool StartReload()
        {
            if (!enabled || _actionsLock.StartReloadLocked())
                return false;

            foreach (var l in subLaunchers)
            {
                if (_ammoSpareQuantity <= 0)
                    return false;

                l.Supply(1);
                _ammoSpareQuantity--;
                if (!l.StartReload())
                {
                    l.Supply(-1);
                    _ammoSpareQuantity++;
                    return false;
                }
            }
            return true;
        }
        internal bool EndReloadForSubLauncher(IMissileLauncher launcher)
        {
            if (launcher.EndReload())
            {
                _ammoQuantityInMagazine++;
                return true;
            }
            launcher.Supply(-1);
            _ammoSpareQuantity++;
            return false;
        }
        public bool EndReload()
        {
            if (!enabled || _actionsLock.EndReloadLocked())
                return false;
            foreach (var l in subLaunchers)
            {
                EndReloadForSubLauncher(l);
            }
            return true;
        }

        public int Supply(int num)
        {
            if (_ammoSpareQuantity + num < 0 || _actionsLock.SupplyLocked())
                return 0;
            var suppNum = Mathf.Min(_definitions.AmmoTotalQuantity - subLaunchers.Length - _ammoSpareQuantity, num);
            _ammoSpareQuantity += (ushort)suppNum;
            return suppNum;
        }

        void OnGUI()
        {
            var style = new GUIStyle();
            style.fontSize = 32;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, 100, 100), _ammoSpareQuantity.ToString(), style);
            GUI.Label(new Rect(0, 100, 100, 100), _ammoQuantityInMagazine.ToString(), style);
            if (Target == null)
                GUI.Label(new Rect(0, 200, 100, 100), "NULL", style);
            else
                GUI.Label(new Rect(0, 200, 100, 100), Target.Obj.name, style);
        }
    }
}
