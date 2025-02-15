using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using System.Data;
using System.Linq;
using Tests;
using Tests.Weapons;
using TMPro;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    [RequireComponent(typeof(X0_MultiMissileLauncherDefines))]
    [RequireComponent(typeof(Animator))]
    public class X0_Animator : MonoBehaviour
    {
        Animator _animator;
        ILauncherAnimatorDefines _defines;
        ILauncherAnimatorActionDefines _actionDefines;
        X0_MultiMissileLauncher _launcher;
        ITimeline _prepareLaunchTimeline;
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _defines = GetComponent<ILauncherAnimatorDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherAnimatorDefines));
            _actionDefines = GetComponent<X0_MultiMissileLauncherDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherAnimatorActionDefines));
            _launcher = GetComponent<X0_MultiMissileLauncher>() ?? throw new ComponentCantFindException(this.gameObject, typeof(X0_MultiMissileLauncher));



        }
        private void OnEnable()
        {
            _launcher.InitializationAction += LauncherInitializeAction;

        }
        private void Start()
        {
            Initialize();
        }
        private void Update()
        {
            _prepareLaunchTimeline.OnUpdate(Time.deltaTime);
        }
        void LauncherInitializeAction(ILauncher l)
        {
            var controller = _animator.runtimeAnimatorController;
            var clips = controller.animationClips;


            ActionInitialize_Reload();

            InitializeSpeedMultiplierForClip(FindClip(clips, _defines.CoverOpenClipName), _defines.CoverOpenSpeedMultiplierName, _actionDefines.CoverOpenOrCloseDuration);
            InitializeSpeedMultiplierForClip(FindClip(clips, _defines.CoverCloseClipName), _defines.CoverCloseSpeedMultiplierName, _actionDefines.CoverOpenOrCloseDuration);
            InitializeSpeedMultiplierForClip(FindClip(clips, _defines.MagazineFullClipName), _defines.MagazineFullSpeedMultiplierName, _actionDefines.MagazineFullOrEmptyDuration);
            InitializeSpeedMultiplierForClip(FindClip(clips, _defines.MagazineEmptyClipName), _defines.MagazineEmptySpeedMultiplierName, _actionDefines.MagazineFullOrEmptyDuration);
            var ml = (IMissileLauncher)l;
            var target = ml.Target;
            var quantity = ml.MagazineCount;
            var reloadTimeline = ml.ReloadTimeline;
            var delayLaunchTimeline = ml.DelayLaunchTimeline;

            ml.TargetChangedAction += TargetChange;


            void InitializeSpeedMultiplierForClip(AnimationClip clip, string multiplierName, float expectedLength)
            {
                var length = clip.length;
                var multiplier = 1f;
                if (expectedLength > 0)
                    multiplier = length / expectedLength;
                _animator.SetFloat(multiplierName, multiplier);
            }
        }
        void LockTarget()
        {
            _animator.SetBool(_defines.TargetLockedParamName, true);
            _prepareLaunchTimeline.Start();
        }
        void UnlockTarget()
        {
            _animator.SetBool(_defines.TargetLockedParamName, false);
            _prepareLaunchTimeline.Stop();
        }
        void TargetChange(IMissileLauncher launcher, ITarget newTarget)
        {
            var actionsLock = launcher.actionsLock;
            var currentTarget = launcher.Target;
            if (currentTarget == null && newTarget != null)
            {
                if (actionsLock.LaunchIsLocked())
                {
                    var timeline = launcher.ReloadTimeline;
                    timeline.EndAction += _ => { LockTarget(); };

                }
                else
                    LockTarget();
            }
            else if (currentTarget != null && newTarget == null)
            {
                if (actionsLock.AnyLocked())
                {
                    var timelines = new ITimeline[] { launcher.ReloadTimeline, launcher.DelayLaunchTimeline };
                    foreach (var t in timelines)
                        if (t.IsRunning)
                        {
                            t.EndAction += _ => { UnlockTarget(); };
                            break;
                        }
                }
                else
                    UnlockTarget();
            }
        }

        AnimationClip FindClip(AnimationClip[] clips, string name)
        {
            var clip = clips.FirstOrDefault(clip => clip.name == name);
            if (clip == null)
                throw new Exception($"Can't find a clip by the name {name}");
            return clip;
        }
        void Initialize()
        {
            _animator.Play(_defines.CoverCloseClipName, 0, 1);
            _animator.Play(_defines.MagazineFullClipName, 1, 1);

            var launcher = (IMissileLauncher)_launcher;



            _prepareLaunchTimeline = new Timeline(_actionDefines.CoverOpenOrCloseDuration);
            _prepareLaunchTimeline.AddPointEvent(0, _ => launcher.actionsLock.LockAll());
            _prepareLaunchTimeline.AddPointEvent(1, _ => launcher.actionsLock.UnlockAll());
        }


        void ActionInitialize_Reload()
        {
            var reloadTimeline = _launcher.ReloadTimeline;
            reloadTimeline.AddPointEvent(0, _ => _animator.SetBool(_defines.ReloadingParamName, true));
        }
    }
}
