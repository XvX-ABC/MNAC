using Assets.Scripts.Utilities.Timeline;
using System;
using Tests.BodyBehaviour.Arm;
using Tests.Utilities;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Weapons
{
    public class Weapon_Test : MonoBehaviour, ILauncher
    {
        [SerializeField]
        string _name;
        [SerializeField]
        WeaponType _type;

        public string Name { get => _name; set => _name = value; }
        public WeaponType Type { get => _type; set => _type = value; }
        Action<ILauncher> ILauncher.InitializationAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ITimeline ILauncher.DelayLaunchTimeline => throw new NotImplementedException();

        ITimeline ILauncher.LaunchDurationTimeline => throw new NotImplementedException();

        ITimeline ILauncher.ReloadTimeline => throw new NotImplementedException();

        ILauncherDefinitions ILauncher.Definitions => throw new NotImplementedException();

        ushort ILauncher.SpareCount => throw new NotImplementedException();

        ushort ILauncher.MagazineCount => throw new NotImplementedException();

        ILauncherActionsLock ILauncher.actionsLock => throw new NotImplementedException();

        bool ILauncher.EndLaunch()
        {
            throw new NotImplementedException();
        }

        bool ILauncher.EndReload()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}