using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm.Weapons
{
    internal class OutputSetting : IOutputSetting
    {
        Playable _parentNode;
        internal int portNum;
        public float Weight
        {
            get => _parentNode.GetInputWeight(portNum);
            set => _parentNode.SetInputWeight(portNum, Mathf.Clamp01(value));
        }
        public OutputSetting(Playable parent, int portNum)
        {
            _parentNode = parent;
            this.portNum = portNum;
        }
    }
}

