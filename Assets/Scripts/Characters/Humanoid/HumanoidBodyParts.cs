using System;
using UnityEngine;

namespace MNAC.Characters.Humanoid
{
    [Serializable]
    internal class HumanoidBodyParts : ICompositeItems
    {
        [Serializable]
        class Mapping
        {
            [SerializeField]
            internal HumanBodyPart part;
            [SerializeField]
            internal GameObject obj;
        }
        [SerializeField]
        Mapping[] _mapping;
        public GameObject GetItem(uint key)
        {
            var idx = Array.FindIndex(_mapping, m => ((uint)m.part) == key);
            if (idx == -1)
                throw new BodyPartNotFoundException($"Can't found a body part by the key '{key}'");
            return _mapping[idx].obj;
        }
    }
}
