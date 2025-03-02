using Assets.Tests.Scripts.Weapons.Assets__v0;
using System;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class X0_AnimatorDefinitions : MonoBehaviour, ILauncherAnimatorDefinitionsEditor
    {
        [Serializable]
        public class Definitions
        {
            public string TargetLockedParamName;
            public string ReloadingParamName;
            public string CoverCloseClipName;
            public string CoverCloseSpeedMultiplierName;
            public string CoverOpenParamName;
            public string CoverOpenClipName;
            public string CoverOpenSpeedMultiplierName;
            public string MagazineFullClipName;
            public string MagazineEmptyParamName;
            public string MagazineFullSpeedMultiplierName;
            public string MagazineEmptyClipName;
            public string MagazineEmptySpeedMultiplierName;
        }
        Definitions _definitions;
        [SerializeField]
        JsonAssetAgent<Definitions> _definitionsAssetAgent;
        public string TargetLockedParamName => _definitions.TargetLockedParamName;

        public string ReloadingParamName => _definitions.ReloadingParamName;

        public string CoverCloseClipName => _definitions.CoverCloseClipName;

        public string CoverCloseSpeedMultiplierName => _definitions.CoverCloseSpeedMultiplierName;

        public string CoverOpenParamName => _definitions.CoverOpenParamName;

        public string CoverOpenClipName => _definitions.CoverOpenClipName;

        public string CoverOpenSpeedMultiplierName => _definitions.CoverOpenSpeedMultiplierName;

        public string MagazineFullClipName => _definitions.MagazineFullClipName;

        public string MagazineEmptyParamName => _definitions.MagazineEmptyParamName;

        public string MagazineFullSpeedMultiplierName => _definitions.MagazineFullSpeedMultiplierName;

        public string MagazineEmptyClipName => _definitions.MagazineEmptyClipName;

        public string MagazineEmptySpeedMultiplierName => _definitions.MagazineEmptySpeedMultiplierName;

        public void Load()
        {
            _definitions = _definitionsAssetAgent.Load() ?? new();
        }

        public void Save()
        {
            _definitionsAssetAgent.Save(_definitions);
        }
    }
}
