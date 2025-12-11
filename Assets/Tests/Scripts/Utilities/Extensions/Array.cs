using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Tests.Extensions
{
    public static class ArrayExtensions
    {
        [Obsolete]
        public static void Append<T>(ref T[] array, T elem)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = elem;
        }
        public static T[] Append<T>(this T[] array, T elem)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = elem;
            return array;
        }
        public static T[] Remove<T>(this T[] array, T elem)
        {
            var index = Array.FindIndex(array, e => e.Equals(elem));
            if (index == -1)
                return array;
            return Remove(array, index);

        }
        public static T[] Remove<T>(this T[] array, int index)
        {
            var length = array.Length;
            if (index == -1 || index >= length)
                throw new IndexOutOfRangeException($"The index '{index}' is out of range '{0} , {array.Length - 1}'");
            if (index != length - 1)
                Array.Copy(array, index + 1, array, index, length - index - 1);
            Array.Resize(ref array, length - 1);
            return array;
        }
    }
}
