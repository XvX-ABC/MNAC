using UnityEngine;

namespace Tests.Locomotion.Animation
{
    public class LocomotionAnimator : MonoBehaviour, IModule
    {
        HorizontalLocomotionAnimator _horizontal;
        JumpLocomotionAnimator _jump;
        AirLocomotionAnimator _air;
        IModule[] _modules;

        private void Awake()
        {
            _horizontal = GetComponent<HorizontalLocomotionAnimator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(HorizontalLocomotionAnimator));
            _jump = GetComponent<JumpLocomotionAnimator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(JumpLocomotionAnimator));
            _air = GetComponent<AirLocomotionAnimator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(AirLocomotionAnimator));

            _modules = new IModule[] { _horizontal, _jump, _air };
        }
        public void OnUpdate(Context context)
        {
            for (int i = 0; i < _modules.Length; i++)
            {
                var m = _modules[i];
                m.OnUpdate(context);
            }
        }
    }
}