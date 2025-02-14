using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using System.Data;
using System.Linq;
using Tests;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    [RequireComponent(typeof(Animator))]
    public class X0_Animator : MonoBehaviour
    {
        Animator _animator;
        ILauncherAnimatorDefines _defines;
        ILauncherAnimatorActionDefines _actionDefines;
        MultiMissileLauncher_X0 _launcher;
        ITimeline _prepareLaunchTimeline;
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _defines = GetComponent<ILauncherAnimatorDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherAnimatorDefines));
            _actionDefines = GetComponent<ILauncherAnimatorActionDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherAnimatorActionDefines));
            _launcher = GetComponent<MultiMissileLauncher_X0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(MultiMissileLauncher_X0));

            Initialize();

        }
        private void OnEnable()
        {
            _launcher.InitializationAction += LauncherInitializeAction;

        }
        private void Start()
        {
        }
        private void Update()
        {
            _prepareLaunchTimeline.OnUpdate(Time.deltaTime);
        }
        void LauncherInitializeAction(ILauncher l)
        {
            var controller = _animator.runtimeAnimatorController;
            var clips = controller.animationClips;


            InitializeAction_PrepareToLaunch(clips);
            InitializeAction_Reload(clips);


            var ml = (IMissileLauncher)l;
            var target = ml.Target;
            var quantity = ml.MagazineCount;
            var reloadTimeline = ml.ReloadTimeline;
            var delayLaunchTimeline = ml.DelayLaunchTimeline;

            ml.TargetChangedAction += TargetChange;



        }
        void StartPrepareLaunch()
        {
            _animator.SetBool(_defines.CoverOpenParamName, true);
            _prepareLaunchTimeline.Start();
        }
        void StopPrepareLaunch()
        {
            _animator.SetBool(_defines.CoverOpenParamName, false);
            _prepareLaunchTimeline.Stop();
        }
        void TargetChange(IMissileLauncher launcher, ITarget newTarget)
        {
            Debug.Log("Target change");
            var actionsLock = launcher.actionsLock;
            var currentTarget = launcher.Target;
            if (currentTarget == null && newTarget != null)
            {
                if (actionsLock.LaunchIsLocked())
                {
                    var timeline = launcher.ReloadTimeline;
                    timeline.EndAction += _ => { StartPrepareLaunch(); };

                }
                else
                    StartPrepareLaunch();
            }
            else if (currentTarget != null && newTarget == null)
            {
                if (actionsLock.AnyLocked())
                {
                    var timelines = new ITimeline[] { launcher.ReloadTimeline, launcher.DelayLaunchTimeline };
                    foreach (var t in timelines)
                        if (t.IsRunning)
                        {
                            t.EndAction += _ => { StopPrepareLaunch(); };
                            break;
                        }
                }
                else
                    StopPrepareLaunch();
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



            _prepareLaunchTimeline = new Timeline(_actionDefines.PrepareLaunch.CoverOpenDuration);
            _prepareLaunchTimeline.AddPointEvent(0, _ => launcher.actionsLock.LockAll());
            _prepareLaunchTimeline.AddPointEvent(1, _ => launcher.actionsLock.UnlockAll());
        }
        void InitializeAction_PrepareToLaunch(AnimationClip[] clips)
        {
            var coverOpenClip = FindClip(clips, _defines.CoverOpenClipName);

            var plDefines = _actionDefines.PrepareLaunch;
            var length = coverOpenClip.length;
            var multiplier = 1f;
            if (plDefines.CoverOpenDuration > 0)
                multiplier = length / plDefines.CoverOpenDuration;
            _animator.SetFloat(_defines.CoverOpenSpeedMultiplierName, multiplier);
        }
        void InitializeAction_Reload(AnimationClip[] clips)
        {
            var reloadTimeline = _launcher.ReloadTimeline;
            InitializeActionOfReloading(clips, reloadTimeline, _defines.CoverCloseClipName, _actionDefines.Reload.CoverCloseProportion, _defines.CoverCloseSpeedMultiplierName, _ => _animator.SetBool(_defines.CoverOpenParamName, false));
            InitializeActionOfReloading(clips, reloadTimeline, _defines.MagazineEmptyClipName, _actionDefines.Reload.MagazineEmptyProportion, _defines.MagazineEmptySpeedMultiplierName, _ => _animator.SetBool(_defines.MagazineEmptyParamName, true));
            InitializeActionOfReloading(clips, reloadTimeline, _defines.MagazineFullClipName, _actionDefines.Reload.MagazineFullProportion, _defines.MagazineFullSpeedMultiplierName, _ => _animator.SetBool(_defines.MagazineEmptyParamName, false));
        }
        void InitializeActionOfReloading(AnimationClip[] clips, ITimeline reloadTimeline, string clipName, Vector2 proportions, string multiplierName, Action<TimelineContext> action)
        {
            var reloadDuration = reloadTimeline.Length;
            var clip = FindClip(clips, clipName);
            var startProportion = proportions.x;
            var endProportion = proportions.y;
            var dv = endProportion - startProportion;

            reloadTimeline.AddPointEvent(startProportion, action);

            var length = clip.length;
            var expectedLength = 0f;
            if (reloadDuration > 1 && dv > 0)
                expectedLength = length / (dv * reloadDuration);

            _animator.SetFloat(multiplierName, expectedLength);
        }
        //void InitializeAction_Reload(AnimationClip[] clips)
        //{
        //    var reloadTimeline = _launcher.ReloadTimeline;

        //    var coverCloseClip = FindClip(clips, _defines.CoverCloseClipName);
        //    //var coverCloseProportion = _defines.CoverClosePlayProportionInReload;
        //    var coverCloseProportion = _actionDefines.Reload.CoverCloseTriggerProportion;

        //    reloadTimeline.AddPointEvent(coverCloseProportion, _ => _animator.SetBool(_defines.CoverOpenParamName, false));

        //    var length = coverCloseClip.length;
        //    var expectedLength = 1f;
        //    if (coverCloseProportion > 0 && _launcher.Defines.ReloadDuration > 0)
        //        expectedLength = length / (coverCloseProportion * _launcher.Defines.ReloadDuration);
        //    _animator.SetFloat(_defines.CoverCloseSpeedMultiplierName, expectedLength);





        //    var magazineEmptyClip = FindClip(clips, _defines.MagazineEmptyClipName);
        //    var magazineEmptyProportion = _actionDefines.Reload.MagazineEmptyTriggerProportion;

        //    reloadTimeline.AddPointEvent(magazineEmptyProportion, _ => _animator.SetBool(_defines.MagazineEmptyParamName, true));

        //    length = magazineEmptyClip.length;
        //    expectedLength = 1f;
        //    if (magazineEmptyProportion > 0 && _launcher.Defines.ReloadDuration > 1)
        //        expectedLength = length / (magazineEmptyProportion * _launcher.Defines.ReloadDuration);
        //    _animator.SetFloat(_defines.MagazineEmptySpeedMultiplierName, expectedLength);





        //    var magazineFullClip = FindClip(clips, _defines.MagazineFullClipName);
        //    //var magazineFullProportion = _defines.CoverClosePlayProportionInReload;
        //    var magazineFullProportion = _actionDefines.Reload.MagazineFullTriggerProportion;

        //    reloadTimeline.AddPointEvent(magazineFullProportion, _ => _animator.SetBool(_defines.MagazineEmptyParamName, false));

        //    length = magazineFullClip.length;
        //    expectedLength = 1f;
        //    if (magazineFullProportion > 1 && _launcher.Defines.ReloadDuration > 1)
        //        expectedLength = length / (magazineFullProportion * _launcher.Defines.ReloadDuration);
        //    _animator.SetFloat(_defines.MagazineFullSpeedMultiplierName, expectedLength);

        //}
    }
}
