using System;
using Tests.Behaviours.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
         internal class AimingAnimator : IDynamicPlayablePart
        {
            AnimationClipPlayable _playable;
            AnimationClip _clip;
            IArmedLauncherArmBehaviourDefinitions _definitions;
            public AimingAnimator(AnimationClip clip, IArmedLauncherArmBehaviourDefinitions definitions)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(_clip));
                _definitions = definitions ?? throw new ArgumentNullException(nameof(_definitions));
            }

            public IOutputSetting OutputSetting { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public Playable GetPlayablePart(PlayableGraph graph)
            {
                if (_playable.IsNull())
                {
                    _playable = AnimationClipPlayable.Create(graph, _clip);
                }
                return _playable;
            }
        }

    }


