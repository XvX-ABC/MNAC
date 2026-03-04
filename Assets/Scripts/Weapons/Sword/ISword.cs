using System;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Weapons.Sword
{
    public interface ISword : IWeapon
    {
        Action<GameObject> HitAction { get; set; }
        GameObject OwnerObj { get; set; }
        Vector3 WorldUp { get; set; }
        float Length { get; }
        LayerMask LayerMaskToHit { get; set; }
        TeamMask TeamMask { get; set; }

        SwordAction GetSwordAction(SwordActionType type);
    }
}