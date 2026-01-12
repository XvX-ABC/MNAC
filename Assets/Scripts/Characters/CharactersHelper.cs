using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters
{
    internal class CharactersHelper
    {
        internal static void DamageCharacter(CharacterBase character, float point)
        {
            if (character == null || character is not IDamageable dc)
                throw new ArgumentNullException(nameof(character));
            var hp = dc.HP;
            hp.ReceivePoint(-point);
        }
        internal static bool KillCharacter(CharacterBase character)
        {
            if (character == null || character is not IDamageable dc)
                throw new ArgumentNullException(nameof(character));
            var hp = dc.HP;
            var point = hp.Point;
            hp.ReceivePoint(-point);
            return true;
        }
        internal static bool RespawnCharacter(CharacterBase character)
        {
            if (character == null || character is not IDamageable dc)
                throw new ArgumentNullException(nameof(character));
            var hp = dc.HP;
            var point = hp.MaxPoint;
            hp.ReceivePoint(point);
            if (!character.enabled)
                character.enabled = true;
            return true;
        }
    }
}
