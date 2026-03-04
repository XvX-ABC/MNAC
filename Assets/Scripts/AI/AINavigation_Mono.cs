using MNAC.Characters;
using MNAC.Characters.Humanoid.Locomotion;
using MNAC.TPhysics.Locomotion;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
namespace MNAC.AI
{
    internal class AINavigation_Mono : AIComponent_Mono
    {
        [SerializeField]
        AINavigation _navigation;
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;

            }
        }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);
            _navigation.Initialize(context);
        }
        public override void Dispose()
        {
            _navigation.Dispose();
            base.Dispose();
        }
        private void OnEnable()
        {
            _navigation.Enabled = true;
        }
        private void OnDisable()
        {
            _navigation.Enabled = false;
        }
    }
}
