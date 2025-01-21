using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MultiLauncher : MonoBehaviour, ILauncher
    {
        ILauncher[] _subLaunchers;
        [SerializeField]
        ushort _ammoSpareQuantity;
        [SerializeField]
        ushort _ammoQuantityInMagazine;
        ILauncherDefines _defines;
        Action<ILauncher> _initializationAction;
        public ushort SpareCount { get => _ammoSpareQuantity; }
        public ushort MagazineCount { get => _ammoQuantityInMagazine; }
        public ILauncherDefines Defines { get => _defines; }
        public Action<ILauncher> InitializationAction { get => _initializationAction; set => _initializationAction = value; }

        void Awake()
        {
            var list = GetComponentsInChildren<ILauncher>().ToList();
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
