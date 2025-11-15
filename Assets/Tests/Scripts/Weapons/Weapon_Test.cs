using System;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Utilities.Timeline;
using Tests.Weapons.Launcher;
using Unity.VisualScripting;
using UnityEngine;

namespace Tests.Weapons
{
    public class Weapon_Test : MonoBehaviour, ILauncher
    {
        [SerializeField]
        string _name;
        [SerializeField]
        WeaponType _type;
        [SerializeField]
        float _reloadDurationTime;
        ITimeline _reloadTimeline;
        public string Name { get => _name; set => _name = value; }
        public WeaponType Type { get => _type; set => _type = value; }
        Action<ILauncher> ILauncher.InitializationAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ITimeline ILauncher.DelayLaunchTimeline => throw new NotImplementedException();

        ITimeline ILauncher.LaunchDurationTimeline => throw new NotImplementedException();

        ITimeline ILauncher.ReloadTimeline => _reloadTimeline;
        ILauncherDefinitions ILauncher.Definitions => throw new NotImplementedException();

        ushort ILauncher.ReservesAmmoCount => throw new NotImplementedException();

        ushort ILauncher.MagazineAmmoCount => throw new NotImplementedException();

        ILauncherActionsLock ILauncher.actionsLock => throw new NotImplementedException();

        public Action<ILauncher> LaunchAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<ILauncher> ReloadAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public GameObject Obj => this.gameObject;

        bool ILauncher.EndLaunch()
        {
            throw new NotImplementedException();
        }

        bool ILauncher.EndReload()
        {
            _reloadTimeline.Pause();
            return true;
            //throw new NotImplementedException();
        }

        int ILauncher.Fill(int num)
        {
            throw new NotImplementedException();
        }

        bool ILauncher.StartLaunch()
        {
            throw new NotImplementedException();
        }

        bool ILauncher.StartReload()
        {
            _reloadTimeline.Restart();
            return true;
            //throw new NotImplementedException();
        }
        void Awake()
        {
            _reloadTimeline = new Timeline(_reloadDurationTime);
        }
        void Update()
        {
            if (_reloadTimeline.IsRunning)
                _reloadTimeline.OnUpdate(Time.deltaTime);
        }

        public void WhenMounted(GameObject mountPoint)
        {
            throw new NotImplementedException();
        }

        public void WhenUnmounted(GameObject mountPoint)
        {
            throw new NotImplementedException();
        }
    }
}