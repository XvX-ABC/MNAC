using Minimalist.Utility.SampleScene;
using System;
using MNAC.Interaction;
using MNAC.Interaction.Influences;
using UnityEngine;

namespace MNAC.Characters.C_0
{
    internal class C_0Accessor : CharacterAccessor<C_0>, IC_0
    {

        public TeamMask TeamMask { get => character.TeamMask; set => character.TeamMask = value; }

        public string Name => character.Name;

        public Guid ID => character.ID;

        public Action<GameObject> DiedAction { get => character.DiedAction; set => character.DiedAction = value; }

        public IKnockback Knockback => character.Knockback;

        public IHealthWithCallBack HP => ((IDamageableWithCallback)character).HP;

        IHealth MNAC.Interaction.IDamageable.HP => HP;

        public GameObject GetItem(uint key)
        {
            return character.GetItem(key);
        }
    }
}
