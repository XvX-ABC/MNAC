using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using Tests.Input;
using Debug = UnityEngine.Debug;

namespace Tests.BodyBehaviour.Arm
{
    [Obsolete]
    public class WeaponHoldingBehaviours_Old : IArmBehaviour
    {
        Dictionary<string, IArmWeaponBehaviour> _weaponBehaviours;
        IArmWeaponBehaviour[] _activedBehaviours;
        WeaponCore _weaponCore;

        IInput IArmBehaviour.Input { set => throw new NotImplementedException(); }

        bool IArmBehaviour.Continuing => throw new NotImplementedException();

        public WeaponHoldingBehaviours_Old(WeaponCore weaponCore, params (string, IArmWeaponBehaviour)[] weaponBehaivours)
        {
            _weaponCore = weaponCore ?? throw new ArgumentNullException(nameof(weaponCore));
            _weaponBehaviours = new();
            foreach (var wb in weaponBehaivours)
            {
                var t = wb.Item1;
                var b = wb.Item2;
                _weaponBehaviours.Add(t, b);
            }

        }

        public WeaponHoldingBehaviours_Old()
        {
        }

        public void ActivateBehaviourBy(string weaponName)
        {
            if (weaponName == null || weaponName.Length == 0)
                throw new ArgumentNullException(nameof(weaponName));
            if (!_weaponBehaviours.TryGetValue(weaponName, out var b))
                throw new CantFindBehaviourByNameException(weaponName);
            if (!_weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new GetWeaponObjByNameFailedException(weaponName);


            Array.Resize(ref _activedBehaviours, _activedBehaviours.Length + 1);
            Array.Copy(_activedBehaviours, 0, _activedBehaviours, 1, _activedBehaviours.Length);
            _activedBehaviours[^1] = b;
        }
        public void UnactivateBehaviourBy(string weaponName)
        {
            if (weaponName == null || weaponName.Length == 0)
                throw new ArgumentNullException(nameof(weaponName));
            for (int i = 0; i < _activedBehaviours.Length; i++)
            {
                var b = _activedBehaviours[i];

            }
        }
        public bool End()
        {
            var result = true;
            foreach (var b in _activedBehaviours)
            {
                var s = b.End();
                if (!s)
                {
                    var name = b.GetType().Name;
                    Debug.LogWarning($"The behaviour '{name}' to end failed.");
                    result = false;
                }
            }
            return result;
        }

        public void OnAnimatorIK(int layerIndex)
        {
            foreach (var b in _activedBehaviours)
                b.OnAnimatorIK(layerIndex);
        }

        public void Update()
        {
            foreach (var b in _activedBehaviours)
                b.Update();
        }

        public bool Begin()
        {
            var result = true;
            foreach (var b in _activedBehaviours)
            {
                var s = b.Begin();
                if (!s)
                {
                    var name = b.GetType().Name;
                    Debug.LogWarning($"The behaviour '{name}' to start failed.");
                    result = false;
                }
            }
            return result;
        }
    }
}
