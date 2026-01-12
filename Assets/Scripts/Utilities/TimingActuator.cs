using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events;
using Tests.Utilities.Timeline.Events.Point;
using UnityEngine;
using UnityEngine.Events;

namespace Tests.Utilities
{
    internal class TimingActuator : MonoBehaviour
    {
        const float INVALID_DURATION = -1;
        [SerializeField]
        float _interval;
        [SerializeField]
        float _duration = INVALID_DURATION;
        [SerializeField]
        bool _isLoop = true;
        //[SerializeField]
        //bool _actuatesEventWhenTimelineStart;
        [SerializeField]
        UnityEvent _events;

        ITimeline _timeline;
        ITimelineEvent _actuatedEvent;
        float _intervalProportion { get => _duration > 0 ? _interval / _duration : 0; }
        private void Awake()
        {
            _timeline = new Timeline.Timeline(_duration, _isLoop);
            var amount = Mathf.FloorToInt(_duration / _interval);
            for (int i = 1; i <= amount; i++)
                _timeline.AddPointEvent(i * _intervalProportion, _ => _events.Invoke());

        }
        private void OnEnable()
        {
            _timeline.Start();
        }
        private void OnDisable()
        {
            _timeline.Pause();
        }
        private void Update()
        {
            _timeline.OnUpdate(Time.deltaTime);
        }
    }
}
