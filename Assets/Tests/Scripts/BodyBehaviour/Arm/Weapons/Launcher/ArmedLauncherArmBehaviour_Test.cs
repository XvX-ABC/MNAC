using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using Tests.Behaviours.Arm.Weapons;
using Tests.Characters;
using Tests.Input;
using Tests.Weapons;
using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class ArmedLauncherArmBehaviour_Test : MonoBehaviour
    {
        [SerializeField]
        WeaponCore _core;
        [SerializeField]
        string _weaponName;
        [SerializeField]
        AnimationClip _idleClip;
        ArmedLauncherArmBehaviour _b;
        [SerializeField]
        Animator _animator;
        [SerializeField]
        AvatarMask _bodyMask;
        [SerializeField]
        AvatarMask _armMask;
        [SerializeField]
        CustomPlayerInput _input;
        IWeapon _weapon;
        PlayableGraph _graph;
        AnimationLayerMixerPlayable mixer;
        Blackboard _blackboard;
        void BlackboardInitialize()
        {
            _blackboard = new();
            _blackboard.TryRegisterField(CharacterBlackboardFields.Input, _input);
        }
        private void Awake()
        {
            BlackboardInitialize();
            _b = GetComponent<ArmedLauncherArmBehaviour>();
            _b.Blackboard = _blackboard;


            _core.TryGetWeaponObj(_weaponName, out var obj);
            _weapon = obj.GetComponent<IWeapon>();
            _graph = PlayableGraph.Create("ArmedLauncher_Test");
        }
        private void Start()
        {
            _b.Weapon = _weapon;
            var p = _b.Animator.GetPlayablePart(_graph);
            var clip = AnimationClipPlayable.Create(_graph, _idleClip);
            mixer = AnimationLayerMixerPlayable.Create(_graph, 2);
            _graph.Connect(clip, 0, mixer, 0);
            _graph.Connect(p, 0, mixer, 1);

            mixer.SetLayerMaskFromAvatarMask(1, _armMask);

            var output = AnimationPlayableOutput.Create(_graph, "Animation", _animator);
            output.SetSourcePlayable(mixer);

            mixer.SetInputWeight(0, 1);
            mixer.SetInputWeight(1, 1);
            var outputSetting = new OutputSetting(mixer, 1);

            _b.OutputSetting = outputSetting;
        }
        private void Update()
        {
        }
        private void OnEnable()
        {
            _graph.Play();
        }
        private void OnDestroy()
        {
            _graph.Destroy();
        }
    }
}
