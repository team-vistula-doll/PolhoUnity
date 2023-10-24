using System;
using UnityEditor;

namespace SerializedObjectUtility
{
    public static class SerializedObjectUtility
    {
        static int PartitionSerializedArray(SerializedProperty arr, IComparable[] keys, int low, int high)
        {
            IComparable pivot = keys[high];
            int i = (low - 1);

            for (int j = low; j <= high - 1; j++)
            {
                if (keys[j].CompareTo(pivot) <= 0)
                {
                    i++;
                    (keys[i], keys[j]) = (keys[j], keys[i]);
                    arr.MoveArrayElement(i, j);
                    arr.MoveArrayElement(j + 1, i + 1);
                }
            }

            (keys[high], keys[i + 1]) = (keys[i + 1], keys[high]);
            arr.MoveArrayElement(high, i + 1);
            arr.MoveArrayElement(i + 2, high + 1);

            return i + 1;
        }

        /// <summary>
        /// Sorts a serialized array/list by a key array
        /// </summary>
        /// <param name="keys">The keys array by which <paramref name="arr"/> will be sorted</param>
        /// <param name="arr">The serialized array with elements not inheriting ScriptableObjects</param>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        public static void SortSerializedPropertyArray(SerializedProperty arr, IComparable[] keys, int start, int end)
        {
            int[] stack = new int[end - start + 1];
            int top = -1;
            stack[++top] = start;
            stack[++top] = end;

            while (top >= 0)
            {
                end = stack[top--];
                start = stack[top--];
                int p = PartitionSerializedArray(arr, keys, start, end);

                if (p - 1 > start)
                {
                    stack[++top] = start;
                    stack[++top] = p - 1;
                }
                if (p + 1 < end)
                {
                    stack[++top] = p + 1;
                    stack[++top] = end;
                }
            }
        }
    }
}