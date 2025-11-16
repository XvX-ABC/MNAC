using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class CursorIndicator : ComponentBase_MonoComponent, ICursorIndicator
    {
        protected RectTransform rectTransform;
        [SerializeField]
        protected float width;
        public virtual float Width
        {
            get => width;
            set
            {
                width = value;
                UpdateWidth();
            }
        }
        public virtual Vector3 CursorPosition
        {
            get => rectTransform.position;
            set
            {
                rectTransform.position = value;
            }
        }
        public override bool Enabled
        {
            get
            {
                if (this == null)
                    return false;
                return this.gameObject.activeSelf;
            }
            set
            {
                if (this != null)
                    this.gameObject.SetActive(value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }
        private void OnValidate()
        {
            if (rectTransform != null)
                UpdateWidth();
        }

        void UpdateWidth()
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
        }

    }
}