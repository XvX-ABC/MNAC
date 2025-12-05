using Tests.Interaction;
using Tests.Utilities.Blackboards;
using UnityEngine;


namespace Tests.Player
{
    internal class GameObjsInScreenCatcher : PlayerComponent
    {
        internal class Filter : ICaughtItemFilter<GameObject>
        {
            public bool CanCatch(GameObject item)
            {
                if (item.TryGetComponent<ITeamInfo>(out var teamInfo))
                {
                    var type = teamInfo.TeamType;
                    return type == TeamType.Enemy;
                }
                else
                    return false;

            }

            public bool CanRelease(GameObject item)
            {
                return true;
            }
        }
        public static implicit operator Interaction.GameObjsInScreenCatcher(GameObjsInScreenCatcher catcher)
        {
            return catcher._catcher;
        }
        Interaction.GameObjsInScreenCatcher _catcher;

        [SerializeField]
        ushort _processingAmountOfFrames = 30;
        [SerializeField]
        Camera _camera;
        [SerializeField]
        bool useCoroutine;


        public ushort ProcessingAmountOfFrames { get => _catcher.ProcessingAmountOfFrames; set => _processingAmountOfFrames = _catcher.ProcessingAmountOfFrames = value; }
        public Camera Camera { get => _catcher.Camera; set => _camera = _catcher.Camera = value; }

        protected override void Awake()
        {
            base.Awake();
        }
        protected virtual void Start()
        {
            if (useCoroutine)
                StartCoroutine(_catcher.UpdateWithCoroutine());
        }
        protected virtual void FixedUpdate()
        {
            if (!useCoroutine)
                _catcher.Update();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<Camera>(BlackboardFields.Camera_Main, out var camera);
            _catcher = new(camera, _processingAmountOfFrames);
            _catcher.AddFilter(new Filter());

            blackboard.TryRegisterField(BlackboardFields.Component_ScreenCatcher, this);
        }
        public override void Dispose()
        {
            blackboard.TryRegisterField(BlackboardFields.Component_ScreenCatcher);
            base.Dispose();
        }
    }
}
