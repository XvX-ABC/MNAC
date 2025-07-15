using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm.Weapons;
using Tests.Weapons;
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
        IWeapon _weapon;
        PlayableGraph _graph;
        private void Awake()
        {
            _b = GetComponent<ArmedLauncherArmBehaviour>();
            _core.TryGetWeaponObj(_weaponName, out var obj);
            _weapon = obj.GetComponent<IWeapon>();
            _graph = PlayableGraph.Create("ArmedLauncher_Test");
        }
        private void Start()
        {
            _b.Weapon = _weapon;
            var p = _b.Animator.GetPlayablePart(_graph);
            var mixer = AnimationMixerPlayable.Create(_graph, 2);
            var clip = AnimationClipPlayable.Create(_graph, _idleClip);

            _graph.Connect(clip, 0, mixer, 0);
            _graph.Connect(p, 0, mixer, 1);

            var output = AnimationPlayableOutput.Create(_graph, "Animation", this.GetComponent<Animator>());
            output.SetSourcePlayable(mixer);
         
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
