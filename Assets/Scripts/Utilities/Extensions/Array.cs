using System;

namespace MNAC.Utilities.Extensions
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
            if (array == null)
            {
                array = new T[] { elem };
                return array;
            }
            Array.Resize(ref array, array.Length + 1);
            array[^1] = elem;
            return array;
        }
        public static T[] Remove<T>(this T[] array, T elem)
        {
            var index = Array.FindIndex(array, e => e.Equals(elem));
            if (index == -1)
                return array;
            return array.Remove(index);

        }
        public static T[] Insert<T>(this T[] array, int index, T elem)
        {
            if (array == null)
            {
                array = new T[] { elem };
                return array;
            }
            Array.Resize(ref array, array.Length + 1);
            if (index != array.Length - 1)
            {
                Array.Copy(array, index, array, index + 1, array.Length - index - 1);
            }
            array[index] = elem;

            return array;
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
