using System;
using Tests.Utilities.Assets;
using UnityEngine;

namespace Tests.Characters.Utilities
{
    [Serializable]
    internal class CharacterPrefab<T> : PrefabLoader<T> where T : Component
    {

    }
}
