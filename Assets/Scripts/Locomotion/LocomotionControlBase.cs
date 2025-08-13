using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using Tests.Environment;
using UnityEngine;

namespace Locomotion
{
    public interface IFrameContext
    {
        public Vector3 TargetPos { get; }
        public Vector3 CurrentPos { get; }
        public Vector3 CurrentVelocity { get; }
        public void Update();
    }
    public interface ILocomotionAnimator
    {
        public void UpdateAnimationOnGround(Vector3 currentVelocity);
        public void UpdateAnimationInJump();
        public void OnFixedUpdate(LocomotionContext context);
    }
    public struct LocomotionContext
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public override string ToString()
        {
            return $"Position: {Position}, Velocity: {Velocity}";
        }
    }
    [Obsolete]
    public abstract class LocomotionControlBase : MonoBehaviour
    {
        protected internal class GravityLocomotion
        {
            Rigidbody _rb;

            public GravityLocomotion(Rigidbody rb)
            {
                _rb = rb;
            }

            public float CalculateVelocityExcludeDrag(float currentVelocity, float deltaTime)
            {
                return CalculateVelocity(currentVelocity, deltaTime) / (1 - deltaTime * _rb.drag);
            }
            public float CalculateVelocity(float currentVelocity, float deltaTime)
            {
                return currentVelocity + Physics.gravity.y * deltaTime;
            }
        }
        [Serializable]
        protected internal class JumpLocomotion
        {

            class PrepareCompleted : PointEvent
            {
                JumpLocomotion _locomotion;

                public PrepareCompleted(float triggerProportion, JumpLocomotion locomotion) : base(triggerProportion)
                {
                    _locomotion = locomotion;
                }


                public override void Execute(TimelineContext context)
                {
                    _locomotion._stepNum = 2;
                }
            }
            class AscendingEvent : RangeEvent
            {
                JumpLocomotion _locomotion;
                float _time;

                public AscendingEvent(float triggeredProportion, float durationProportion, JumpLocomotion locomotion) : base(durationProportion, triggeredProportion)
                {
                    _locomotion = locomotion;
                }

                public override void Execute(TimelineContext context)
                {
                    _locomotion._currentVelocity = _locomotion.CalculateVelocityInAscendingStage(_time);
                    _time += Time.fixedDeltaTime;
                }
                public override void Reset()
                {
                    _time = 0;
                }
            }
            class AscendingStageEndEvent : PointEvent
            {
                JumpLocomotion _locomotion;

                public AscendingStageEndEvent(float triggeredProportion, JumpLocomotion locomotion) : base(triggeredProportion)
                {
                    _locomotion = locomotion;
                }

                public override void Execute(TimelineContext context)
                {
                    _locomotion._stepNum = 3;
                }
            }
            [SerializeField]
            float _ascendingDuration;
            [SerializeField]
            float _duration;
            [SerializeField]
            float _time;
            float _startVelocity;
            public bool InJumping;
            IJumpDefinitions _definition;
            IGroundDetector _groundSampler;
            float _currentVelocity;
            byte _stepNum;
            Timeline _timeline;
            GravityLocomotion _gravityLocomotion;
            float _contraryDragVelocity;
            Action<JumpLocomotion> _jumpStartActions;
            Action<JumpLocomotion> _jumpEndActions;
            public JumpLocomotion(IJumpDefinitions definition, IGroundDetector groundSampler, GravityLocomotion gravityLocomotion)
            {
                _definition = definition;
                _groundSampler = groundSampler;
                _gravityLocomotion = gravityLocomotion;
                Initialize();
                var p0 = _definition.PreparationDuration / _ascendingDuration;
                var p1 = (1 - p0);
                _timeline = new(_ascendingDuration, false, new PrepareCompleted(p0, this), new AscendingEvent(p0, p1, this), new AscendingStageEndEvent(1, this));
            }

            //bool _isOnGround => _groundSampler.TouchedGround;


            public float AscendingDuration { get => _ascendingDuration; }
            [Obsolete]
            public float Duration { get => _duration; }
            public float Timer { get => _timeline.Time; }
            public float StartVelocity { get => _startVelocity; }
            public float CurrentVelocity { get => _currentVelocity; }
            public byte StepNum { get => _stepNum; }
            public bool ReachedMaxHeight { get => _stepNum > 1 && _time >= _ascendingDuration; }
            public Timeline Timeline { get => _timeline; }
            public void Initialize()
            {
                _startVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * _definition.Height);
                _ascendingDuration = _startVelocity / -Physics.gravity.y + _definition.PreparationDuration;
                _duration = _ascendingDuration * 2;
            }
            public void RegisterStartAction(Action<JumpLocomotion> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));
                _jumpStartActions += action;
            }
            public void UnregisterStartAction(Action<JumpLocomotion> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));
                _jumpStartActions -= action;
            }
            public void StartJump()
            {
                if (!InJumping)
                {
                    _time = 0;
                    InJumping = true;
                    _stepNum = 1;
                    _timeline.Restart();
                    _jumpStartActions?.Invoke(this);
                }
            }
            public void EndJump()
            {
                if (InJumping)
                {
                    _time = 0;
                    InJumping = false;
                    _currentVelocity = 0;
                    _stepNum = 0;
                    _timeline.Pause();
                    _jumpEndActions?.Invoke(this);
                }
            }
            public bool ShouldEndJump()
            {
                //return _isOnGround && ReachedMaxHeight;
                return false;
            }
            float CalculateVelocityInAscendingStage(float time)
            {
                return _startVelocity + Physics.gravity.y * time;
            }

            public void OnFixedUpdate(float currentVerticalVelocity)
            {
                if (ShouldEndJump())
                    EndJump();
                else
                {
                    _timeline.OnUpdate(Time.fixedDeltaTime);
                    if (_stepNum >= 3)
                        _currentVelocity = 0;
                    else
                        _time = _timeline.Time;
                }
            }
        }
        protected internal class BaseLocomotion
        {
            IGroundDetector _groundSampler;
            IBaseDefinitions _definition;
            GravityLocomotion _gravityLocomotion;
            bool _isAscending;
            bool _inAir;
            float _ascendingVelocity;
            float _currentVelocityInAir;
            Transform _trans;
            public float AscendingVelocity
            {
                get => _ascendingVelocity;
            }
            public float CurrentVelocityInAir
            {
                get => _currentVelocityInAir;
            }
            public bool IsAscending
            {
                get => _isAscending;
                set => _isAscending = value;
            }
            public bool InAir
            {
                get => _inAir;
                set => _inAir = value;
            }
            public BaseLocomotion(IGroundDetector groundSampler, IBaseDefinitions definition, Transform trans, GravityLocomotion gravityLocomotion)
            {
                _groundSampler = groundSampler;
                _definition = definition;
                _ascendingVelocity = definition.AscendingSpeed;
                _trans = trans;
                _gravityLocomotion = gravityLocomotion;
            }

            public Vector3 CalculateHorizontalVelocityOnGround(Vector3 direction, Vector3 currentVelocity)
            {
                //return CalculateHorizontalVelocity(direction, currentVelocity, _groundSampler.Normal);
                return default;
            }
            public Vector3 CalculateHorizontalVelocityInAir(Vector3 direction, Vector3 currentVelocity)
            {
                return CalculateHorizontalVelocity(direction, currentVelocity, _trans.up);
            }
            public Vector3 CalculateHorizontalVelocity(Vector3 direction, Vector3 currentVelocity, Vector3 planeNormal)
            {
                direction = direction.normalized;
                Vector3 velocity;
                var speed = _definition.MaxSpeed;
                var accelerationSpeed = _definition.AccelerationSpeed;
                if (planeNormal != Vector3.zero)
                    direction = Vector3.ProjectOnPlane(direction, planeNormal);
                //velocity.x = Mathf.MoveTowards(currentVelocity.x, direction.x * speed, accelerationSpeed);
                //velocity.z = Mathf.MoveTowards(currentVelocity.z, direction.z * speed, accelerationSpeed);
                //velocity.y = currentVelocity.y;
                velocity = Vector3.MoveTowards(currentVelocity, direction * speed, accelerationSpeed);
                return velocity;
            }
            public void StartAscending()
            {
                _isAscending = true;
            }
            public void StopAscending()
            {
                _isAscending = false;
            }
            public float CalculateVerticalVelocityInAir(float currentVelocity)
            {
                //return IsAscending ? _ascendingVelocity : _gravityLocomotion.CalculateVelocity(currentVelocity, Time.fixedDeltaTime);
                return IsAscending ? _ascendingVelocity : 0;
            }
            public void OnFixedUpdate(float currentVerticalVelocity)
            {
                _currentVelocityInAir = CalculateVerticalVelocityInAir(currentVerticalVelocity);
            }
        }
        protected internal class QuarterViewRotation
        {
            IFrameContext _frameContext;
            Rigidbody _rb;
            GameObject _obj;

            public QuarterViewRotation(IFrameContext frameContext, Rigidbody rb, GameObject obj)
            {
                _frameContext = frameContext;
                _rb = rb;
                _obj = obj;
            }

            public Quaternion Rotate()
            {
                var currentPos = _frameContext.CurrentPos;
                var targetPos = _frameContext.TargetPos;
                var towards = (targetPos - currentPos);
                return Rotate(towards);
            }
            Quaternion Rotate(Vector3 towards)
            {
                towards.y = 0;
                towards = towards.normalized;
                var currentRotation = _rb.rotation;
                //towards.y = 0;
                var finalVector = Quaternion.Inverse(currentRotation) * towards;
                var newRotation = currentRotation * Quaternion.LookRotation(finalVector, _obj.transform.up);
                return newRotation;
            }
        }
        protected internal class QuickBoostLocomotion
        {
            IBoostingDefinitions _definition;
            class EndBoostEvent : PointEvent
            {
                QuickBoostLocomotion _locomotion;

                public EndBoostEvent(float triggeredProportion, QuickBoostLocomotion locomotion) : base(triggeredProportion)
                {
                    _locomotion = locomotion;
                }
                public override void Execute(TimelineContext context)
                {
                    _locomotion.EndBoost();
                }
            }
            public QuickBoostLocomotion(IBoostingDefinitions definition)
            {
                _definition = definition;
                _lastBoostTime = float.MaxValue;
                _timeline = new Timeline(_definition.Duration, false, new EndBoostEvent(1f, this));
            }

            bool _boosting;
            float _timer;
            Vector3 _currentVelocity;
            Vector3 _startVelocity;
            Timeline _timeline;
            float _lastBoostTime;
            public bool Boosting
            {
                get => _boosting;
            }
            public Vector3 CurrentVelocity
            {
                get => _currentVelocity;
            }
            public Timeline Timeline
            {
                get => _timeline;
            }
            public void StartBoost()
            {
                if (!_boosting)
                {
                    _timer = 0;
                    var currentTime = Time.unscaledTime;
                    _boosting = Mathf.Abs(currentTime - _lastBoostTime) >= _definition.Interval;
                    if (_boosting)
                        _timeline.Restart();
                }
            }
            public void EndBoost()
            {
                if (_boosting)
                {
                    _timeline.Pause();
                    _boosting = false;
                    _startVelocity = Vector3.zero;
                    _lastBoostTime = Time.unscaledTime;
                }
            }
            public Vector3 CalculateVelocity()
            {
                var hvelocity = _startVelocity;
                //hvelocity *= _definition.Velocity;
                hvelocity.y = 0;
                return hvelocity;
            }
            public Vector3 CalculateVelocity(Vector3 direction, Vector3 currentVelocity)
            {
                direction = direction.normalized;
                //var velocity = direction * _definition.Velocity;
                var velocity = Vector3.zero;
                velocity.y = currentVelocity.y;
                return velocity;
            }
            public void OnFixedUpdate()
            {
                if (_timer != 0 && _timer >= _definition.Duration)
                    EndBoost();
                else if (_boosting)
                    _timer += Time.fixedDeltaTime;
            }
        }
        protected Rigidbody rb;
        protected Vector3 _currentNormal;
        [SerializeField]
        protected LayerMask _terrainLayerMask;
        [SerializeField]
        protected internal JumpLocomotion jumpLocomotion;
        protected internal QuarterViewRotation rotation;
        protected internal BaseLocomotion baseLocomotion;
        protected internal QuickBoostLocomotion boostLocomotion;
        protected internal GravityLocomotion gravityLocomotion;
        internal ILocomotionDefinitions locomotionDefinition;
        IGroundDetector _groundSampler;
        ILocomotionAnimator _animator;
        //bool _isOnGround { get => _groundSampler.TouchedGround; }
        internal abstract IFrameContext CurrentFrameContext { get; }
        internal abstract bool IsForward { get; }
        internal abstract bool IsBack { get; }
        internal abstract bool IsRight { get; }
        internal abstract bool IsLeft { get; }
        internal abstract bool IsAscending { get; }
        internal abstract bool TriggeredQuickBoost { get; }
        protected void Awake()
        {
            LoadRequirementComponents();
            InitializeLocomotionObjs();
        }
        void Start()
        {

        }
        protected void LoadRequirementComponents()
        {
            locomotionDefinition = GetComponent<ILocomotionDefinitions>() ?? throw new NullReferenceException("");
            _groundSampler = GetComponent<IGroundDetector>() ?? throw new NullReferenceException("");
            _animator = GetComponent<ILocomotionAnimator>() ?? throw new NullReferenceException("");
            //_groundSampler.AutoSample = false;
            rb = GetComponent<Rigidbody>();
            rb.drag = 0;
            rb.useGravity = true;
        }
        protected void InitializeLocomotionObjs()
        {
            gravityLocomotion = new(rb);
            jumpLocomotion = new(locomotionDefinition.Jump, _groundSampler, gravityLocomotion);
            baseLocomotion = new(_groundSampler, locomotionDefinition.Base, this.transform, gravityLocomotion);
            rotation = new(CurrentFrameContext, rb, this.gameObject);
            boostLocomotion = new(locomotionDefinition.Boosting);
        }
        protected void Update()
        {
            CurrentFrameContext.Update();
        }
        protected void FixedUpdate()
        {
            //_groundSampler.Sample();

            rb.MoveRotation(rotation.Rotate());
            var currentVelocity = rb.velocity;
            var expectedVelocity = Vector3.zero;
            var direction = GetHorizontalInputDirection();

            if (TriggeredQuickBoost)
            {
                boostLocomotion.StartBoost();
                //boostLocomotion.Timeline.Start();
            }
            else
            {
                if (IsAscending)
                {
                    if (!jumpLocomotion.InJumping)
                    {
                        //if (_isOnGround)
                        if(false)
                        {
                            jumpLocomotion.StartJump();
                        }
                        else
                            baseLocomotion.StartAscending();
                    }
                    else if ((jumpLocomotion.InJumping && jumpLocomotion.ReachedMaxHeight) || (jumpLocomotion.InJumping && jumpLocomotion.StepNum == 1 && baseLocomotion.InAir))
                    //else if (jumpLocomotion.InJumping)
                    {
                        jumpLocomotion.EndJump();
                        baseLocomotion.StartAscending();
                    }
                }
                else
                {
                    baseLocomotion.StopAscending();
                }
            }
            //if (_isOnGround)
            if(false)
            {
                if (direction != Vector3.zero)
                    //expectedVelocity = baseLocomotion.CalculateHorizontalVelocityOnGround(direction, currentVelocity);
                    expectedVelocity = boostLocomotion.Boosting ? boostLocomotion.CalculateVelocity(direction, currentVelocity) : baseLocomotion.CalculateHorizontalVelocityOnGround(direction, currentVelocity);
                baseLocomotion.InAir = false;
            }
            else
            {
                if (direction != Vector3.zero)
                    //expectedVelocity = baseLocomotion.CalculateHorizontalVelocityInAir(direction, currentVelocity);
                    expectedVelocity = boostLocomotion.Boosting ? boostLocomotion.CalculateVelocity(direction, currentVelocity) : baseLocomotion.CalculateHorizontalVelocityInAir(direction, currentVelocity);
                baseLocomotion.InAir = true;
            }
            var pos = this.transform.position;
            Debug.DrawLine(pos, pos + expectedVelocity.normalized * 4, Color.white);
            if (boostLocomotion.Boosting)
            {
                //boostLocomotion.OnFixedUpdate();
                boostLocomotion.Timeline.OnUpdate(Time.fixedDeltaTime);
            }
            else if (jumpLocomotion.InJumping)
            {
                //jumpLocomotion.Timeline.OnUpdate(Time.fixedDeltaTime);
                jumpLocomotion.OnFixedUpdate(currentVelocity.y);
                expectedVelocity.y = jumpLocomotion.CurrentVelocity;
            }
            else if (baseLocomotion.InAir)
            {

                baseLocomotion.OnFixedUpdate(currentVelocity.y);
                expectedVelocity.y = baseLocomotion.CurrentVelocityInAir;
            }

            if (expectedVelocity.x == 0 && expectedVelocity.z == 0)
            {
                //rb.velocity = currentVelocity * (1 - Time.fixedDeltaTime * locomotionDefinition.Base.Drag);
                expectedVelocity.x = CalculateDefaultVelocityWithDrag(currentVelocity.x);
                expectedVelocity.z = CalculateDefaultVelocityWithDrag(currentVelocity.z);
            }
            if (expectedVelocity.y == 0)
                expectedVelocity.y = currentVelocity.y;

            rb.velocity = expectedVelocity;
            float CalculateDefaultVelocityWithDrag(float currentVelocity)
            {
                return currentVelocity * (1 - Time.fixedDeltaTime * locomotionDefinition.Base.Drag);
            }

            var context = new LocomotionContext()
            {
                Position = rb.position,
                Rotation = rb.rotation,
                Velocity = rb.velocity,
            };
            _animator.OnFixedUpdate(context);
        }

        Vector3 GetHorizontalInputDirection()
        {
            var direction = Vector3.zero;
            if (IsForward)
                direction = Vector3.forward;
            else if (IsBack)
                direction = Vector3.back;

            if (IsRight)
                direction += Vector3.right;
            else if (IsLeft)
                direction += Vector3.left;
            return direction;
        }
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;
            var pos = this.transform.position;
            var velocity = rb.velocity;
            //var normal = _groundSampler.Normal;
            var normal = Vector3.zero;
            Gizmos.DrawLine(pos, pos + this.transform.forward * 3);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + velocity.normalized * 3);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + normal * 3);
        }
    }
}
