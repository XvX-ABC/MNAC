using System;
using Tests.Weapons;

namespace Tests.Behaviours.Arms.Weapons
{
    internal class DefaultWeaponSelector
    {
        int _index = -1;
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
