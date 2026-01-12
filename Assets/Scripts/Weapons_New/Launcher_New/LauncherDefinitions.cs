using System;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    public class LauncherDefinitions : MonoBehaviour, ILauncherDefinitions
    {
        [SerializeField]
        ushort _ammoReserveAmount;
        [SerializeField]
        ushort _ammoInMagazineAmount;
        [SerializeField]
        float _launchingIntervalTime;
        [SerializeField]
        float _reloadDurationTime;
        [SerializeField]
        Vector2 _launchDelayRange;
        [SerializeField]
        bool _allowedAutoReload;


        public ushort AmmoInMagazineAmount => _ammoInMagazineAmount;

        public float ReloadDurationTime => _reloadDurationTime;


        public Vector2 LaunchDelayRange => _launchDelayRange;

        public ushort AmmoReserveAmount => _ammoReserveAmount;

        public float LaunchingIntervalTime => _launchingIntervalTime;

        public bool AllowedAutoReload { get => _allowedAutoReload; }
    }
}
