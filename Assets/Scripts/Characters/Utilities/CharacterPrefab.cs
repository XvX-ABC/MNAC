using System;
using MNAC.Utilities.Assets;
using UnityEngine;

namespace MNAC.Characters.Utilities
{
    [Serializable]
    internal class CharacterPrefab<T> : PrefabLoader<T> where T : Component
    {

    }
}
