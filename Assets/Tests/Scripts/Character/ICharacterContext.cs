using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

namespace Tests.Characters
{
    public interface ICharacterContext
    {
        public IInput Input { get; }
        public Animator Animator { get; }
        public ReadOnlyDictionary<string, ICharacterComponent> Components { get; }
        public bool RegisterComponent(ICharacterComponent component);
        public bool UnregisterComponent(ICharacterComponent component);
    }
    public interface IArmedCharacterContext : ICharacterContext
    {
        public WeaponCore WeaponCore { get; }
    }

}
