using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MultiLauncher : MonoBehaviour, IMissileLauncher
    {
        class TimelinesGroup : ITimeline
        {
            ITimeline[] _timelines;

            public TimelinesGroup(params ITimeline[] timelines)
            {
                _timelines = timelines;
            }

            public bool IsRunning => _timelines.Any(t => t.IsRunning);

            public float Time => throw new NotImplementedException();

            public bool AddEvent(ITimelineEvent evt)
            {
                foreach (var t in _timelines)
                    if (!t.AddEvent(evt))
                    {
                        RemoveEvent(evt);
                        return false;
                    }
                return true;
            }

            public void Continue()
            {
                foreach (var l in _timelines)
                    l.Continue();
            }

            public void OnUpdate(float deltaTime)
            {
                foreach (var l in _timelines)
                    l.OnUpdate(deltaTime);
            }

            public bool RemoveEvent(ITimelineEvent evt)
            {
                foreach (var t in _timelines)
                    if (!t.RemoveEvent(evt))
                        return false;
                return true;
            }

            public void Start()
            {
                foreach (var l in _timelines)
                    l.Start();
            }

            public void Stop()
    {
                foreach (var l in _timelines)
                    l.Stop();
            }
        }
        IMissileLauncher[] _subLaunchers;
        [SerializeField]
        ushort _ammoSpareQuantity;
        [SerializeField]
        ushort _ammoQuantityInMagazine;
        IMissileLauncherDefines _defines;
        Action<ILauncher> _initializationAction;
        ITarget _target;
        ITimeline _reloadTimeline;
        ITimeline _delayLaunchTimeline;
        public ushort SpareCount { get => _ammoSpareQuantity; }
        public ushort MagazineCount { get => _ammoQuantityInMagazine; }
        public ILauncherDefines Defines { get => _defines; }
        IMissileLauncherDefines IMissileLauncher.Defines => _defines;
        public Action<ILauncher> InitializationAction { get => _initializationAction; set => _initializationAction = value; }
        public ITarget Target
        {
            get => _target;
            set
            {
                _target = value;
                foreach (var l in _subLaunchers)
                    l.Target = value;
            }
        }


        public ITimeline ReloadTimeline { get => _reloadTimeline; }
        public ITimeline DelayLaunchTimeline { get => _delayLaunchTimeline; }


        void Awake()
        {
            var list = GetComponentsInChildren<IMissileLauncher>().ToList();
            if (list.Contains(this))
                list.Remove(this);
            _subLaunchers = list.ToArray();
            _defines = GetComponent<IMissileLauncherDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherDefines));


            foreach (var l in _subLaunchers)
                l.InitializationAction += launcher => launcher.Supply(-(launcher.Defines.AmmoSpareQuantity - 1));


            Target = GetComponent<ITarget>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ITarget));
        }
        void Start()
        {
            _ammoSpareQuantity = (ushort)(_defines.AmmoTotalQuantity - _subLaunchers.Length);

            var quantity = _subLaunchers.Length;
            var reloadTimelines = new ITimeline[quantity];
            var delayLaunchTimelines = new ITimeline[quantity];
            for (int i = 0; i < quantity; i++)
            {
                var l = _subLaunchers[i];
                reloadTimelines[i] = l.ReloadTimeline;
                delayLaunchTimelines[i] = l.DelayLaunchTimeline;
            }


            _reloadTimeline = new TimelinesGroup(reloadTimelines);
            _delayLaunchTimeline = new TimelinesGroup(delayLaunchTimelines);


            _initializationAction?.Invoke(this);
        }
        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
                Launch();
            if (Input.GetKeyDown(KeyCode.R))
                StartReload();
            if (Input.GetKeyDown(KeyCode.Space))
                Supply(10);
        }
        public void Launch()
        {
            if (!enabled)
                return;
            foreach (var l in _subLaunchers)
                l.Launch();
            _ammoQuantityInMagazine -= (ushort)_subLaunchers.Length;
        }

        public bool StartReload()
        {
            if (!enabled)
                return false;
            foreach (var l in _subLaunchers)
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
        public bool EndReload()
        {
            if (!enabled)
                return false;
            foreach (var l in _subLaunchers)
            {
                if (!l.EndReload())
                {
                    l.Supply(-1);
                    _ammoSpareQuantity++;
                    return false;
                }
                _ammoQuantityInMagazine++;
            }
            return true;
        }

        public int Supply(int num)
        {
            if (_ammoSpareQuantity + num < 0)
                return 0;
            var suppNum = Mathf.Min(_defines.AmmoTotalQuantity - _subLaunchers.Length - _ammoSpareQuantity, num);
            _ammoSpareQuantity += (ushort)suppNum;
            return suppNum;
        }
    }
}
