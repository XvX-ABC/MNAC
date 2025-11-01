using System;
using System.Data;
using System.Linq;
using Tests.Utilities.Timeline;
using Tests.Weapons.Launcher;
using Tests.Weapons.MissileLauncher;
using Tests.Weapons.MultiMissileLauncher;
using TMPro;
using UnityEngine;
using Tests.Utilities.Timeline.Events.Point;

namespace Tests.Weapons.MultiMissileLauncher.Animation
{
    [RequireComponent(typeof(Animator))]
    public class X0_Animator : MonoBehaviour
    {
        Animator _animator;
        IX0_MultiMissileLauncherAnimatorDefinitions _definition;
        IX0_MultiMissileLauncherActionDefinitions _actionDefinition;
        X0_MultiMissileLauncher _launcher;
        //ITimeline _prepareLaunchTimeline;
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _definition = GetComponent<IX0_MultiMissileLauncherAnimatorDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IX0_MultiMissileLauncherAnimatorDefinitions));
            _actionDefinition = GetComponent<IX0_MultiMissileLauncherActionDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IX0_MultiMissileLauncherActionDefinitions));
            _launcher = GetComponent<X0_MultiMissileLauncher>() ?? throw new ComponentCantFindException(gameObject, typeof(X0_MultiMissileLauncher));



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
            //_prepareLaunchTimeline.OnUpdate(Time.deltaTime);
        }
        void LauncherInitializeAction(ILauncher l)
        {
            var controller = _animator.runtimeAnimatorController;
            var clips = controller.animationClips;


            ActionInitialize_Reload();

            InitializeSpeedMultiplierForClip(FindClip(clips, _definition.CoverOpenClipName), _definition.CoverOpenSpeedMultiplierName, _actionDefinition.CoverOpenOrCloseDuration);
            InitializeSpeedMultiplierForClip(FindClip(clips, _definition.CoverCloseClipName), _definition.CoverCloseSpeedMultiplierName, _actionDefinition.CoverOpenOrCloseDuration);
            InitializeSpeedMultiplierForClip(FindClip(clips, _definition.MagazineFullClipName), _definition.MagazineFullSpeedMultiplierName, _actionDefinition.MagazineFullOrEmptyDuration);
            InitializeSpeedMultiplierForClip(FindClip(clips, _definition.MagazineEmptyClipName), _definition.MagazineEmptySpeedMultiplierName, _actionDefinition.MagazineFullOrEmptyDuration);
            var ml = (IMissileLauncher)l;
            var target = ml.Target;
            var quantity = ml.MagazineAmmoCount;
            var reloadTimeline = ml.ReloadTimeline;
            var delayLaunchTimeline = ml.DelayLaunchTimeline;

            ml.TargetChangeAction += TargetChange;


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
            _animator.SetBool(_definition.TargetLockedParamName, true);
            //_prepareLaunchTimeline.Start();
        }
        void UnlockTarget()
        {
            _animator.SetBool(_definition.TargetLockedParamName, false);
            //if (_prepareLaunchTimeline.IsRunning)
            //    _prepareLaunchTimeline.Stop();

        }
        void TargetChange(IMissileLauncher launcher, ITarget_Obsolete newTarget)
        {
            var actionsLock = launcher.actionsLock;
            var currentTarget = launcher.Target;
            if (currentTarget == null && newTarget != null)
            {
                LockTarget();
            }
            else if (currentTarget != null && newTarget == null)
            {
                //if (actionsLock.AnyLocked() && !_prepareLaunchTimeline.IsRunning)
                var ltimeline = launcher.LaunchDurationTimeline;
                var dtimeline = launcher.DelayLaunchTimeline;
                if (ltimeline.IsRunning || dtimeline.IsRunning)
                {
                    ltimeline.EndAction += EndAction;
                    void EndAction(TimelineContext _)
                    {
                        Debug.Log("Unlocked the target");
                        UnlockTarget();
                        ltimeline.EndAction -= EndAction;
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
            _animator.Play(_definition.CoverCloseClipName, 0, 1);

            //var launcher = (IMissileLauncher)_launcher;



            //_prepareLaunchTimeline = new Timeline(_actionDefinition.CoverOpenOrCloseDuration);
            //_prepareLaunchTimeline.AddPointEvent(0, _ => launcher.actionsLock.LockAll());
            //_prepareLaunchTimeline.AddPointEvent(1, _ => launcher.actionsLock.UnlockAll());
        }


        void ActionInitialize_Reload()
        {
            var reloadTimeline = _launcher.ReloadTimeline;
            reloadTimeline.AddPointEvent(0, _ => { _animator.SetTrigger(_definition.ReloadingParamName); Debug.Log("Enter the reload trigger"); });
        }
    }
}
