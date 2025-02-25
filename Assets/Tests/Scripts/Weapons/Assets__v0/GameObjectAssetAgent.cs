using Assets.Tests.Scripts.Weapons.Assets__0;
using System;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    [Serializable]
    public class GameObjectAssetAgent : AssetAgentBase<GameObject>
    {
#if UNITY_EDITOR
        public GameObjectAssetAgent() : base(new GameObjectAssetLoader(), new GameObjectAssetSaver())
        {
        }
#else
        public GameObjectAssetAgent() : base(new GameObjectAssetLoader())
        {

        }
#endif

    }
}
