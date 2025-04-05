using UnityEngine;

namespace Tests.Locomotion.Animation
{
    public class LocomotionAnimator : MonoBehaviour, IModule
    {
        HorizontalLocomotionAnimator _horizontal;
        JumpLocomotionAnimator _jump;
        AirLocomotionAnimator _air;
        BoostingLocomotionAnimator _boosting;
        IModule[] _modules;

        private void Awake()
        {
            _horizontal = GetComponent<HorizontalLocomotionAnimator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(HorizontalLocomotionAnimator));
            _jump = GetComponent<JumpLocomotionAnimator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(JumpLocomotionAnimator));
            _air = GetComponent<AirLocomotionAnimator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(AirLocomotionAnimator));
            _boosting = GetComponent<BoostingLocomotionAnimator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(BoostingLocomotionAnimator));
            _modules = new IModule[] { _horizontal, _jump, _air, _boosting };
        }
        public void OnFixedUpdate(Context context)
        {
            for (int i = 0; i < _modules.Length; i++)
            {
                var m = _modules[i];
                m.OnFixedUpdate(context);
            }
        }
    }
}