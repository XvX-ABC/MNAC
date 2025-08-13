using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm.Weapons
{
    internal class OutputSetting : IOutputSetting
    {
        internal Playable parentPart;
        internal int portNum;
        public float Weight
        {
            get => parentPart.GetInputWeight(portNum);
            set => parentPart.SetInputWeight(portNum, Mathf.Clamp01(value));
        }
        public OutputSetting(Playable parent, int portNum)
        {
            this.parentPart = parent;
            this.portNum = portNum;
        }
    }
}

