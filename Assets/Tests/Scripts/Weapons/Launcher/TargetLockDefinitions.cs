using System;
using UnityEngine;

namespace Tests.Weapons.Launcher
{
    [Serializable]
    public class TargetLockDefinitions:ITargetLockDefinitions
    {
        [Range(0,1)]
        [SerializeField]
        float _radius;
        public float ViewPortRadius
        {
            get => _radius;
        }
    }
}