using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    [Serializable]
    public class JumpDefinitions : IJumpDefinitions
    {
        [SerializeField]
        float _height;
        public float Height => _height;

    }
}
