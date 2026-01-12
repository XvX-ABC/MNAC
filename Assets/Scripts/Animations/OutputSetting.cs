using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Animations
{
    internal class OutputSetting : IOutputSetting
    {
        internal IAnimationPlayablePart parent;
        internal int portNum;
        float _weight;
        public OutputSetting(IAnimationPlayablePart parent, int portNum)
        {
            this.parent = parent;
            this.portNum = portNum;
        }
        internal OutputSetting() : this(null, -1)
        {

        }

        //public float Weight
        //{
        //    get => parent.PlayablePart.GetInputWeight(portNum);
        //    //get => _weight;
        //    set
        //    {
        //        var v = Mathf.Clamp01(value);
        //        //_weight = v;
        //        parent.PlayablePart.SetInputWeight(portNum, v);
        //    }
        //}
        public float Weight
        {
            get => _weight;
            set
            {
                var v = Mathf.Clamp01(value);
                _weight = v;
                if (parent != null && !parent.PlayablePart.IsNull() && portNum > -1)
                    parent.PlayablePart.SetInputWeight(portNum, _weight);
            }
        }

        public int PortNum { get => portNum; set => portNum = value; }
        public IAnimationPlayablePart Parent { get => parent; set => parent = value; }
    }
}
