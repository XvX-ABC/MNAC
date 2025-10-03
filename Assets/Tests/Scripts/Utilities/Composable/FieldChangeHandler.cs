using Tests.Utilities.Blackboards;

namespace Tests.Utilities.Composable
{
    public class FieldChangeHandler : FieldChangeHandler<object, object>
    {
        public FieldChangeHandler(object key) : base(key)
        {
        }
    }

}
