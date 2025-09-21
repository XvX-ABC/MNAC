using System.Linq;

namespace Tests.Interaction.Influence
{
    public class InfluenceReceivingCore
    {
        IInfluenceReceptor[] _receptors;
        public InfluenceReceivingCore(params IInfluenceReceptor[] receptors)
        {
            this._receptors = receptors;
        }
        public T FindReceptor<T>() where T : IInfluenceReceptor
        {
            return (T)_receptors.FirstOrDefault(r => r is T);
        }
        public IInfluenceReceptor FindReceptor(string name)
        {
            return _receptors.FirstOrDefault(r => r.Name == name);
        }
        public void Update()
        {
            foreach (var r in _receptors)
                if (r != null && r.Enabled)
                    r.Update();
        }
    }
}
