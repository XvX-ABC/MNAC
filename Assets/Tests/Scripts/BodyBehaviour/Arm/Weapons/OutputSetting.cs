using UnityEngine.Playables;

namespace Tests.Behaviours.Arm.Weapons
{
    internal class OutputSetting : IOutputSetting
    {
        Playable _parentNode;
        int _portNum;
        public float Weight
        {
            get => _parentNode.GetInputWeight(_portNum);
            set => _parentNode.SetInputWeight(_portNum, value);
        }
        public OutputSetting(Playable parent, int portNum)
        {
            _parentNode = parent;
            _portNum = portNum;
        }
    }
}

