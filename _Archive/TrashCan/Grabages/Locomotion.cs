using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal class FourDirectionInputEvent : CustomEvent<Locomotion.Context, Vector3>
    {
        public Func<bool> ForwardTrigger;
        public Func<bool> BackTrigger;
        public Func<bool> LeftTrigger;
        public Func<bool> RightTrigger;
        Vector3 CalculateDirectionOnPlane(Vector3 expectedDirection, Vector3 normalOnPlane)
        {
            var normal = normalOnPlane;
            if (normal == Vector3.up || normal == Vector3.zero)
                return expectedDirection;
            return Vector3.ProjectOnPlane(expectedDirection, normal);
        }
        public override Vector3 Invoke(Locomotion.Context context)
        {
            var direction = Vector3.zero;
            var normal = context.NormalOnGround;

            if (!Locomotion.Context.CheckNormalIsValid(normal))
                normal = Vector3.up;

            var trans = context.Obj.transform;

            if (ForwardTrigger())
                direction = CalculateDirectionOnPlane(trans.forward, normal);
            else if (BackTrigger())
                direction = CalculateDirectionOnPlane(-trans.forward, normal);


            if (LeftTrigger())
                direction += CalculateDirectionOnPlane(-trans.right, normal);
            else if (RightTrigger())
                direction += CalculateDirectionOnPlane(trans.right, normal);


            return direction;
        }
    }
    internal class ValidDirectionInputEvent : CustomEvent<Vector3, bool>
    {
        public override bool Invoke(Vector3 direction)
        {
            return direction != Vector3.zero;
        }
    }
    [RequireComponent(typeof(BasicLocomotion))]
    [RequireComponent(typeof(JumpLocomotion))]
    [RequireComponent(typeof(QuickBoostLocomotion))]
    internal partial class Locomotion : MonoBehaviour
    {
        JumpLocomotion _jump;
        BasicLocomotion _base;
        QuickBoostLocomotion _quickBoost;
        Context _context;
        void Awake()
        {
            _jump = GetComponent<JumpLocomotion>();
            _base = GetComponent<BasicLocomotion>();
            _quickBoost = GetComponent<QuickBoostLocomotion>();
            _context = new(this.gameObject);


            var body = _context.RigidBody;
            body.rotation = Quaternion.identity;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


            _base.Context = _context;

            _jump.Context = _context;

            _quickBoost.Context = _context;
            _quickBoost.Base = _base;

        }
        void FixedUpdate()
        {
            _context.OnFixedUpdateStart();
            _base.OnUpdate();
            _jump.OnUpdate();
            _quickBoost.OnUpdate();
            _context.OnFixedUpdateEnd();
        }

    }
}
