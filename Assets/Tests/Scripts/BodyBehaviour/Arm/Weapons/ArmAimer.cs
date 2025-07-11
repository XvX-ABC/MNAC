using RootMotion.FinalIK;
using System;
using Tests.BT;
using Tests.Input;


namespace Tests.Behaviours.Arm
{
    public class ArmAimer : /*IArmBehaviour*/ IAimer
    {
        AimIK _ik;
        ITarget _target;
        bool _endabled;
        public ArmAimer(AimIK ik)
        {
            this._ik = ik ?? throw new NullReferenceException(nameof(ik));
        }

        public IInput Input { set { } }

        public bool Continuing => _endabled;
        public ITarget Target { get => _target; set => _target = value; }
        public float Weight
        {
            get => _ik.solver.IKPositionWeight;
            set => _ik.solver.IKPositionWeight = value;
        }
        public bool OnExit()
        {
            if (_target == null)
            {
                return false;
            }
            _ik.solver.IKPositionWeight = 0f;
            _endabled = false;
            return true;
        }

        public bool OnEnter()
        {
            if (_target == null)
            {
                return false;
            }
            _ik.solver.IKPositionWeight = 1f;
            _endabled = true;
            return true;

        }
        public override void OnStop()
        {
            _ik.solver.IKPositionWeight = 0f;
        }
        protected override TaskState OnWork()
        {
            if (_target == null)
                return TaskState.Failure;
            else
            {
                _ik.solver.IKPositionWeight = 1f;
            }
            _ik.solver.IKPosition = _target.Position;
            return TaskState.Running;

        }
        public void OnUpdate()
        {
            if (!_endabled)
                return;
            _ik.solver.IKPosition = _target.Position;
        }
    }
}
