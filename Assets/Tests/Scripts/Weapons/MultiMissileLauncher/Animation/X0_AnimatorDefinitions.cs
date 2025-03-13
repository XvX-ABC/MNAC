using System;
using System.Diagnostics;
using UnityEngine;

namespace Tests.Weapons.MultiMissileLauncher.Animation
{
    public class X0_AnimatorDefinitions : MonoBehaviour, IX0_MultiMissileLauncherAnimatorDefinitions
    {
        [SerializeField]
        string _coverCloseClipName;
        [SerializeField]
        string _coverCloseSpeedMultiplierName;

        [SerializeField]
        string _coverOpenClipName;
        //[SerializeField]
        //string _coverOpenParamName;
        [SerializeField]
        string _coverOpenSpeedMultiplierName;

        [SerializeField]
        string _magazineFullClipName;
        [SerializeField]
        string _magazineFullSpeedMultiplierName;

        [SerializeField]
        string _magazineEmptyClipName;
        //[SerializeField]
        //string _magazineEmptyParamName;
        [SerializeField]
        string _magazineEmptySpeedMultiplierName;

        [SerializeField]
        string _targetLockedParamName;
        [SerializeField]
        string _reloadingParamName;

        public string CoverCloseClipName { get => _coverCloseClipName; }
        public string CoverCloseSpeedMultiplierName { get => _coverCloseSpeedMultiplierName; }
        public string CoverOpenClipName { get => _coverOpenClipName; }
        public string CoverOpenParamName { get => throw new NotImplementedException(); }
        public string CoverOpenSpeedMultiplierName { get => _coverOpenSpeedMultiplierName; }
        public string MagazineFullClipName { get => _magazineFullClipName; }
        public string MagazineFullSpeedMultiplierName { get => _magazineFullSpeedMultiplierName; }
        public string MagazineEmptyClipName { get => _magazineEmptyClipName; }
        public string MagazineEmptyParamName { get => throw new NotImplementedException(); }
        public string MagazineEmptySpeedMultiplierName { get => _magazineEmptySpeedMultiplierName; }

        public string TargetLockedParamName => _targetLockedParamName;

        public string ReloadingParamName => _reloadingParamName;
    }
}
