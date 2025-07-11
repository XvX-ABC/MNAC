using Tests.Assets;
using UnityEngine;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Animations
{
    public class ArmAnimationDefinitions : MonoBehaviour, IArmAnimationDefinitions
    {
        [SerializeField]
        JsonAssetAgent_Managed<ArmSwitchingAnimationDefinitions> _switching;
        public IArmSwitchingAnimationDefinitions Switching
        {
            get
            {
                if (_switching.Asset == null)
                    _switching.Load();
                return _switching.Asset;
            }
        }
    }
}
