using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.TPhysics.Locomotion;
using MNAC.Utilities.Extensions;
using UnityEngine;

namespace MNAC.Characters.Interaction
{
    internal class SphericalObjsTrigger : MNAC.Interaction.SphericalObjsTrigger, IDisposable
    {
        #region internal classes
        class AngleChecker : EvaluationModuleBase
        {
            Vector3 _origin;
            Vector3 _forward;
            Plane _plane;
            float _maxAngle;
            public AngleChecker(float maxAngle)
            {
                _maxAngle = maxAngle;
            }

            public Vector3 Origin { get => _origin; set => _origin = value; }
            public float MaxAngle { get => _maxAngle; set => _maxAngle = value; }

            public bool Check(GameObject target)
            {
                var localPos = target.transform.position - _origin;
                var p = Vector3.ProjectOnPlane(localPos, _plane.normal);
                var angle = Vector3.Angle(p, _forward);
                return angle <= _maxAngle;

            }
            public override Context Update(Context context)
            {
                _forward = context.Forward;
                _plane = context.WorldPlane;
                return base.Update(context);
            }
        }
        #endregion
        LocomotionCore _locomotionCore;
        AngleChecker _angleChecker;
        public void Initialize(MNAC.TPhysics.Locomotion.LocomotionCore locomotionCore, float maxAngle)
        {
            _angleChecker = new(maxAngle);
            _locomotionCore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            _locomotionCore.EvaluationModules = _locomotionCore.EvaluationModules.Append(_angleChecker);
            this.enabled = true;
            _angleChecker.Enabled = true;
        }
        void Start()
        {
            if (_angleChecker == null)
                this.enabled = false;
        }
        void Update()
        {
            _angleChecker.Origin = this.transform.position;
        }
        protected override void OnTriggerEnter(Collider other)
        {
            if (_angleChecker != null && !_angleChecker.Check(other.gameObject))
                return;
            base.OnTriggerEnter(other);
        }
        protected override void OnTriggerStay(Collider other)
        {
            if (_angleChecker != null && !_angleChecker.Check(other.gameObject))
                return;
            base.OnTriggerStay(other);
        }
        protected override void OnTriggerExit(Collider other)
        {
            if (_angleChecker != null && !_angleChecker.Check(other.gameObject))
                return;
            base.OnTriggerExit(other);
        }
        void OnDestroy()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_locomotionCore != null)
                _locomotionCore.EvaluationModules = _locomotionCore.EvaluationModules.Remove(_angleChecker);
        }

    }
}
