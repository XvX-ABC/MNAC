using Tests.Environment;
using UnityEngine;
namespace Tests.Locomotion_Obsolete
{
    public class QuarterViewRotation : IModule
    {
        GameObject _obj;

        public QuarterViewRotation(GameObject obj)
        {
            _obj = obj;
        }


        void Rotate(Context context)
        {
            var target = context.Target;
            var currentPos = context.Position;
            var targetPos = target.Position;

            var towards = (targetPos - currentPos);
            towards = Vector3.ProjectOnPlane(towards, Vector3.up).normalized;
            var currentRotation = Quaternion.LookRotation(_obj.transform.forward);
            var finalVector = Quaternion.Inverse(currentRotation) * towards;    
            var newRotation = currentRotation * Quaternion.LookRotation(finalVector, Vector3.up);
            context.Rotation = newRotation;
        }
        public void OnFixedUpdate(Context context)
        {
            Rotate(context);
        }

    }
}