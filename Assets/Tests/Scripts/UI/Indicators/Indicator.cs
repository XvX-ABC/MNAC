using DG.Tweening;
using System;
using UnityEngine;

namespace Tests.UI
{
    public abstract class Indicator : UIComponent, IIndicator
    {

        [Serializable]
        public struct Animation
        {
            [Range(1, 10)]
            [SerializeField]
            public float DiffusionProportion;
        }
        [SerializeField]
        protected float width;
        [SerializeField]
        protected float animationDuration;
        [SerializeField]
        protected Animation animationDefinitions;
        protected RectTransform rectTransform;

        public virtual float Width
        {
            get => width;
            set
            {
                width = value;
                UpdateWidth();
            }
        }
        public virtual float AnimationDuration { get => animationDuration; set => animationDuration = value; }
        public virtual Animation AnimationDefinitions { get => animationDefinitions; set => animationDefinitions = value; }
        public Vector3 LocalPosition { get => rectTransform.localPosition; set => rectTransform.localPosition = value; }

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }
        protected virtual void Start()
        {
            UpdateWidth();
        }
        protected virtual void OnEnable()
        {
            if (animationDuration > 0 && animationDefinitions.DiffusionProportion >= 1)
            {
                DOTween.To(() => width * animationDefinitions.DiffusionProportion, x => Width = x, width, animationDuration);

            }
        }
        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Width = width;
#endif
        }
        void UpdateWidth()
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
        }
    }
}
