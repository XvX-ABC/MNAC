using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Characters;
using UnityEngine;

namespace Tests.Players
{
    internal class Player : MonoBehaviour
    {
        CharacterComponent[] _components;
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _components = GetComponentsInChildren<CharacterComponent>();
        }
    }
}
