using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Animations
{
    [Obsolete]
    internal class OutputSetting_Obsolete : IOutputSetting
    {
        internal Playable parentPart;
        internal int portNum;
        float _weight;
        public float Weight
        {
            //get => parentPart.GetInputWeight(portNum);
            //set => parentPart.SetInputWeight(portNum, Mathf.Clamp01(value));
            get => _weight;
            set
            {
                _weight = Mathf.Clamp01(value);
                parentPart.SetInputWeight(portNum, _weight);
            }
        }

        public int PortNum { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IAnimationPlayablePart Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public OutputSetting_Obsolete(Playable parent, int portNum)
        {
            parentPart = parent;
            this.portNum = portNum;
        }
    }
}

