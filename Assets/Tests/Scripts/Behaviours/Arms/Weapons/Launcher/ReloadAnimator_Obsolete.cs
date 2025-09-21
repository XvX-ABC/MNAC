using System;
using Tests.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    [Obsolete]
        internal class ReloadAnimator_Obsolete : IDynamicPlayablePart
        {
            AnimationClipPlayable _playable;
            AnimationClip _clip;
            float _speed;
            public ITimeline ReloadTimeline
            {
                set
                {
                    if (value == null)
                        throw new NullReferenceException(nameof(value));
                    _speed = value.Length == 0 ? 1 : _clip.length / value.Length;
                    if (!_playable.Equals(default))
                        _playable.SetSpeed(_speed);
                }
            }

            public IOutputSetting OutputSetting { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public ReloadAnimator_Obsolete(AnimationClip clip)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            }

            public Playable GetPlayablePart(PlayableGraph graph)
            {
                if (_playable.IsNull())
                {
                    _playable = AnimationClipPlayable.Create(graph, _clip);
                    _playable.SetSpeed(_speed);
                }
                return _playable;
            }
            public void Play()
            {
                _playable.SetTime(0);
                _playable.Play();
            }
            public void Stop()
            {
                _playable.Pause();
            }
        }

}

