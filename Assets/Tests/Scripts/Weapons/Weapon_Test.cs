using System;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Utilities.Timeline;
using Tests.Weapons.Launcher;
using Unity.VisualScripting;
using UnityEngine;

namespace Tests.Weapons
{
    public class Weapon_Test : MonoBehaviour, ILauncher_Obsolete
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
        Action<ILauncher_Obsolete> ILauncher_Obsolete.InitializationAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ITimeline ILauncher_Obsolete.DelayLaunchTimeline => throw new NotImplementedException();

        ITimeline ILauncher_Obsolete.LaunchDurationTimeline => throw new NotImplementedException();

        ITimeline ILauncher_Obsolete.ReloadTimeline => _reloadTimeline;
        ILauncherDefinitions ILauncher_Obsolete.Definitions => throw new NotImplementedException();

        ushort ILauncher_Obsolete.ReservesAmmoCount => throw new NotImplementedException();

        ushort ILauncher_Obsolete.MagazineAmmoCount => throw new NotImplementedException();

        ILauncherActionsLock ILauncher_Obsolete.actionsLock => throw new NotImplementedException();

        public Action<ILauncher_Obsolete> LaunchAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<ILauncher_Obsolete> ReloadAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public GameObject Obj => this.gameObject;

        bool ILauncher_Obsolete.EndLaunch()
        {
            throw new NotImplementedException();
        }

        bool ILauncher_Obsolete.EndReload()
        {
            _reloadTimeline.Pause();
            return true;
            //throw new NotImplementedException();
        }

        int ILauncher_Obsolete.Fill(int num)
        {
            throw new NotImplementedException();
        }

        bool ILauncher_Obsolete.StartLaunch()
        {
            throw new NotImplementedException();
        }

        bool ILauncher_Obsolete.StartReload()
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