using Unity.VisualScripting;
using UnityEngine;
namespace MNAC.AI
{
    internal class AIRotationControl_Mono : AIComponent_Mono
    {
        AIRotationControl _controller;
        protected override void Awake()
        {
            base.Awake();
            _controller = new();
        }
        private void Update()
        {
            _controller.Update();
        }
        private void OnEnable()
        {
            _controller.Enabled = true;
        }
        private void OnDisable()
        {
            _controller.Enabled = false;
        }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);
            _controller.Initialize(context);
        }
        public override void Dispose()
        {
            _controller.Dispose();
            base.Dispose();
        }
    }
}
