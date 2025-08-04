using System;

namespace Tests.Behaviours.Arm
{
    internal class DefaultWeaponSelector
    {
        ushort _index;
        public string Select(WeaponDescription[] originDefinitions)
        {
            if (originDefinitions == null)
                throw new ArgumentNullException(nameof(originDefinitions));


            var length = originDefinitions.Length;
            var newIndex = _index + 1;
            if (newIndex >= length)
                newIndex = 0;
            _index = (ushort)newIndex;


            return originDefinitions[_index].Name;
        }
    }
}
