using System;

namespace Tests.Weapons.MultiMissileLauncher.Animation
{
    public interface IX0_MultiMissileLauncherAnimatorDefinitions
    {

        public string TargetLockedParamName { get; }
        public string ReloadingParamName { get; }

        public string CoverCloseClipName { get; }
        public string CoverCloseSpeedMultiplierName { get; }

        [Obsolete]
        public string CoverOpenParamName { get; }
        public string CoverOpenClipName { get; }
        public string CoverOpenSpeedMultiplierName { get; }

        public string MagazineFullClipName { get; }
        [Obsolete]
        public string MagazineEmptyParamName { get; }
        public string MagazineFullSpeedMultiplierName { get; }

        public string MagazineEmptyClipName { get; }
        public string MagazineEmptySpeedMultiplierName { get; }
    }
}
