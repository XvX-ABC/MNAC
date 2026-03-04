using MNAC.Utilities.Attributes;
using MNAC.Utilities.Blackboards;
using UnityEngine;


namespace MNAC.Characters.Interaction
{
    [PlayerComponent(DontDestroyOnLoad = true)]
    internal class GameObjsInScreenCatcher : CharacterComponent
    {
        MNAC.Interaction.GameObjsInScreenCatcher _catcher;

        [SerializeField]
        ushort _processingAmountOfFrames = 30;
        [SerializeField]
        Camera _camera;
        //[SerializeField]
        //bool useCoroutine;

        public ushort ProcessingAmountOfFrames { get => _catcher.ProcessingAmountOfFrames; set => _processingAmountOfFrames = _catcher.ProcessingAmountOfFrames = value; }
        public Camera Camera { get => _catcher.Camera; set => _camera = _catcher.Camera = value; }

        protected override void Awake()
        {
            base.Awake();
        }
        protected virtual void Start()
        {
            //if (useCoroutine)
                //StartCoroutine(_catcher.UpdateWithCoroutine());
        }
        protected virtual void FixedUpdate()
        {
            //if (!useCoroutine)
            //    _catcher.Update();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Player_Camera_Main, out var camera);
            _catcher = new(camera, _processingAmountOfFrames);

            blackboard.TryRegisterField(CharacterBlackboardFields.Player_ScreenCatcher, this);
        }
    }
}
