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
        public Blackboard() : base(new FieldChangeHandler(CharacterBlackboardFields.FieldChangeHandler))
        {
        }

        protected Blackboard(params IMiddleware<object, object>[] middlewares) : base(middlewares)
        {
            this.middlewares.Append(new FieldChangeHandler(CharacterBlackboardFields.FieldChangeHandler));
        }

    }
}
