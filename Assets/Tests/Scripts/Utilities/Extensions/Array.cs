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
        public static void Append<T>(ref T[] array, T elem)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = elem;
        }
        public static T[] Append_D<T>(T[] array, T elem)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = elem;
            return array;
        }

        [Obsolete]
        public static void Append<T>(this T[] array, T elem)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = elem;
        }
        public static bool Remove<T>(this T[] array, T elem)
        {
            var length = array.Length;
            var index = Array.IndexOf(array, elem);
            return Remove(array, index);
        }
        public static bool Remove<T>(this T[] array, int index)
        {
            var length = array.Length;
            if (index == -1 || index >= length)
                return false;
            if (index != length - 1)
                Array.Copy(array, index + 1, array, index, length - index - 1);
            Array.Resize(ref array, length - 1);
            return true;
        }
    }
}
