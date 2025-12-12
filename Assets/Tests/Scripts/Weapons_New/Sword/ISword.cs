using System;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    public interface ISword : IWeapon
    {
        Action<GameObject> HitAction { get; set; }
        GameObject OwnerObj { get; set; }
        Vector3 WorldUp { get; set; }
        float Length { get; }
        LayerMask LayerMaskToHit { get; set; }

        SwordAction GetSwordAction(SwordActionType type);
    }
}