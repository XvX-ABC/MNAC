using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.UI
{

    public class IndicatorsManager : UIComponent
    {
        [SerializeField]
        List<IndicatedTarget> _targets;
        [SerializeField]
        BoxIndicator _indicatorPrefab;
        [SerializeField]
        Camera _camera;
        [SerializeField]
        int _handleAmountInCoroutine;
        RectTransform _rectTransform;
        public List<IndicatedTarget> Targets { get => _targets; set => _targets = value; }
        public Camera Camera { get => _camera; set => _camera = value; }

        protected override void Awake()
        {
            base.Awake();
            BoxIndicator.InitializeObjPool(_indicatorPrefab, this.transform);
            _rectTransform = this.GetComponent<RectTransform>();
        }
        private void Start()
        {
            //StartCoroutine(DrawTargets());
        }
        private void LateUpdate()
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                if (target.IsValid)
                {

                    var spos = target.GetScreenPosition(_camera);

                    var indicator = target.Indicator;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, spos, null, out var localPos))
                        indicator.LocalPosition = localPos;
                }
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException(UIBlackboardFields.Camera_Main, out _camera);
            blackboard.TryRegisterField(UIBlackboardFields.Indicators_Manager, this);
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(UIBlackboardFields.Indicators_Manager);
        }
        internal void AddTarget(IndicatedTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (_targets.Contains(target))
                return;
            target.Indicator = GetIndicator(target.IndicatorType);
            _targets.Add(target);
        }
        internal void RemoveTarget(IndicatedTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (!_targets.Contains(target))
                return;
            if (target.Indicator)
                ReleaseIndicator(target.Indicator);
            target.Indicator = null;
            _targets.Remove(target);
        }
        public T AddTargetFor<T>(GameObject obj) where T : IndicatedTarget
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var target = obj.GetComponent<T>();
            if (!target)
                target = obj.AddComponent<T>();
            target.Manager = this;
            return target;
        }
        public void RemoveTargetFor<T>(GameObject obj) where T : IndicatedTarget
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var target = obj.GetComponent<T>();
            if (target)
                GameObject.Destroy(target);
        }
        IEnumerator DrawTargets()
        {
            while (true)
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    if (!this.enabled)
                        break;
                    var target = _targets[i];
                    if (target.IsValid)
                    {

                        var spos = target.GetScreenPosition(_camera);

                        var indicator = target.Indicator;
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, spos, null, out var localPos))
                            indicator.LocalPosition = localPos;
                    }
                    if (_handleAmountInCoroutine <= 0 || i % _handleAmountInCoroutine == 0)
                        yield return null;
                }
                yield return null;
            }
        }
        Indicator GetIndicator(IndicatorType type) => type switch
        {
            IndicatorType.Box => BoxIndicator.GetInstance()
        };
        void ReleaseIndicator(Indicator instance)
        {
            if (instance is BoxIndicator indicator)
                BoxIndicator.ReleaseInstance(indicator);
        }
    }
}
