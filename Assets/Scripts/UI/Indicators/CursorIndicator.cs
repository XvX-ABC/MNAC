using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    [RequireComponent(typeof(RectTransform))]
    /*
     * TODO: CursorIndicator的位置最好由组件自行更新，而不是由外部脚本控制
     */
    public class CursorIndicator : UIComponent, ICursorIndicator
    {
        protected RectTransform rectTransform;
        [SerializeField]
        RectTransform _parent;
        [SerializeField]
        protected float width;
        [SerializeField]
        Camera _camera;
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
                //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, value, _camera, out Vector2 localPoint))
                //{
                //    rectTransform.localPosition = localPoint;
                //}
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

        public Camera Camera { get => _camera; set => _camera = value; }

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
            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
            var wh = rectTransform.rect.size;
            var v = width <= 0 ? 0 : Mathf.Max(wh.x, wh.y) / width;
            rectTransform.localScale = new Vector3(v, v, v);
        }

    }
}