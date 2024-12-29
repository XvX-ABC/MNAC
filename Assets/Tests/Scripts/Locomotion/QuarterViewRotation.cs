using UnityEngine;
namespace Tests.Locomotion
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
            towards.y = 0;
            towards = towards.normalized;
            var currentRotation = context.Rotation;
            //towards.y = 0;
            var finalVector = Quaternion.Inverse(currentRotation) * towards;
            var newRotation = currentRotation * Quaternion.LookRotation(finalVector, _obj.transform.up);
            context.Rotation = newRotation;
        }
        public void Update(Context context)
        {
            Rotate(context);
        }
    }
}