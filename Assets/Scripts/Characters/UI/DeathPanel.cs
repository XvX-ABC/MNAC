using DG.Tweening;
using MNAC.UI;
using MNAC.Utilities.Blackboards;
using UnityEngine;
using UnityEngine.UI;

namespace MNAC.Characters.UI
{
    internal class DeathPanel : UIComponent
    {
        [SerializeField]
        float _transitionDuration;
        CanvasGroup _group;
        bool _hide;

        public bool Hide
        {
            get => _hide;
            set
            {
                DOTween.To(() => _group.alpha, x => _group.alpha = x, value ? 0 : 1, _transitionDuration);
                _hide = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _group = GetComponent<CanvasGroup>();
            _group.alpha = 0;
            _hide = true;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterUIBlackboardFields.Death_Panel, this);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterUIBlackboardFields.Death_Panel);
            base.Dispose();
        }

    }
}
