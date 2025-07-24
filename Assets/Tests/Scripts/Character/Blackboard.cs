using System;
using System.Linq;
using Tests.Blackboards;

namespace Tests.Characters
{
    public class Blackboard : Blackboard<object, object>
    {
        protected FieldChangeHandler<object, object> handler
        {
            get => (FieldChangeHandler<object, object>)this.middlewares[0];
        }
        public Blackboard() : base(new FieldChangeHandler<object, object>(CharacterBlackboardFields.FieldChangeHandler))
        {
        }

        protected Blackboard(params IMiddleware<object, object>[] middlewares) : base(middlewares)
        {
            this.middlewares.Append(new FieldChangeHandler<object, object>(CharacterBlackboardFields.FieldChangeHandler));
        }
        public void TrackValue<T>(object key, Action<T, T> action)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            var handler = this.handler;
            handler.RegisterAction(key, (et, ov, nv) =>
            {
                if (ov != nv && ov is T v)
                    action(v, (T)nv);
            });
        }
      
    }
}
