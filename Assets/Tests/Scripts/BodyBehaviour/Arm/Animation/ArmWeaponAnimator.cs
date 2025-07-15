using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using Tests.Extensions;
using Tests.Weapons;
using TMPro;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm.Animations
{
    //internal class ArmWeaponBehavioursAnimator
    //{
    //    Dictionary<string, IArmWeaponHoldingBehavioursAnimator> _weaponAnimators;
    //    IArmWeaponHoldingBehavioursAnimator[] _activatedAnimators;
    //    public ArmWeaponBehavioursAnimator(Animator animator, ArmWeaponHoldingBehaviours behaviours, (string, IArmWeaponHoldingBehavioursAnimator)[] animationMappings)
    //    {
    //        _weaponAnimators = new();
    //        foreach (var am in animationMappings)
    //        {
    //            var name = am.Item1;
    //            var wa = am.Item2;
    //            wa.Animator = animator;
    //            wa.OnAwake();
    //            _weaponAnimators.Add(name, wa);
    //        }
    //        behaviours.ActivatedAction = w => ActivateAnimationBy(w);
    //        behaviours.UnactivatedAction = w => UnactivateAnimationBy(w);
    //    }
    //    void ActivateAnimationBy(IWeapon weapon)
    //    {
    //        var name = weapon.Name;
    //        if (_weaponAnimators.TryGetValue(name, out var wa))
    //        {
    //            if (_activatedAnimators == null)
    //                _activatedAnimators = new IArmWeaponHoldingBehavioursAnimator[] { wa };
    //            else
    //                _activatedAnimators.Append(wa);
    //            wa.OnEnter();
    //        }
    //        else
    //        {
    //            //throw new CantFindBehaviourByNameException(name);
    //        }
    //    }
    //    void UnactivateAnimationBy(IWeapon weapon)
    //    {
    //        var name = weapon.Name;
    //        if (_weaponAnimators.TryGetValue(name, out var animator))
    //        {
    //            if (_activatedAnimators.Length == 1)
    //                _activatedAnimators = null;
    //            else
    //                _activatedAnimators.Remove(animator);
    //            animator.OnExit();
    //        }
    //        else
    //        {

    //        }
    //    }
    //    public void OnUpdate()
    //    {
    //        if (_activatedAnimators == null)
    //            return;
    //        for (var i = 0; i < _activatedAnimators.Length; i++)
    //        {
    //            var animator = _activatedAnimators[i];
    //            if (animator == null)
    //                continue;
    //            animator.OnUpdate();
    //        }
    //    }
    //}
    //internal class ArmWeaponAnimator

    //{
    //    IArmAnimationDefinitions _definitions;
    //    ArmWeaponSwitching _switching;
    //    ArmWeaponBehavioursAnimator _behavioursAnimator;
    //    Animator _animator;
    //    public ArmWeaponAnimator(ArmCore core, Animator animator, IArmAnimationDefinitions definitions, (string, IArmWeaponHoldingBehavioursAnimator)[] animatorMappings)
    //    {
    //        _definitions = definitions;
    //        _switching = core.weaponSwitching;
    //        _animator = animator;
    //        _behavioursAnimator = new(animator, core.holdingBehaviours, animatorMappings);
    //        InitializeSwitchingAnimation();
    //    }
    //    void InitializeSwitchingAnimation()
    //    {
    //        var currentLength = _switching.Timeline.Length;
    //        var multiplier = currentLength <= 0 ? 1f : _definitions.Switching.ClipLength / currentLength;
    //        _animator.SetFloat(_definitions.Switching.MultiplierName, multiplier);
    //        _switching.EntryAction += () =>
    //        {
    //            _animator.SetTrigger(_definitions.Switching.EnterName);
    //        };
    //    }
    //    public void OnUpdate()
    //    {
    //        _behavioursAnimator.OnUpdate();
    //    }
    //}
}
