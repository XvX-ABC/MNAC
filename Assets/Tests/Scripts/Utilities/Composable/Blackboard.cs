using System;
using System.Linq;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;

namespace Tests.Utilities.Composable
{
    public class Blackboard : Blackboard<object, object>
    {
        protected FieldChangeHandler<object, object> handler
        {
            get => (FieldChangeHandler<object, object>)this.middlewares[0];
        }
        public Blackboard() : base(new FieldChangeHandler(MiddlewareFields.FieldChangeHandler))
        {
        }

        protected Blackboard(params IMiddleware<object, object>[] middlewares) : base(middlewares)
        {
            this.middlewares.Append(new FieldChangeHandler(MiddlewareFields.FieldChangeHandler));
        }
        public void TryReadValueOrThrowException<T>(object key, out T value)
        {
            if (!TryReadValue<T>(key, out value))
                throw new Exception($"Key '{key}' not found in blackboard.");
        }

    }
}
