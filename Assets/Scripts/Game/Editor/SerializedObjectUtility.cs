using System;
using UnityEditor;

namespace SerializedObjectUtility
{
    public static class SerializedObjectUtility
    {
        static int PartitionSerializedArray(SerializedProperty arr, int low, int high)
        {
            IComparable pivot = (IComparable)arr.GetArrayElementAtIndex(high).managedReferenceValue;
            int i = (low - 1);

            for (int j = low; j <= high - 1; j++)
            {
                if ((arr.GetArrayElementAtIndex(j).managedReferenceValue as IComparable).CompareTo(pivot) <= 0)
                {
                    i++;

                    arr.MoveArrayElement(i, j);
                }
            }

            arr.MoveArrayElement(i + 1, high);

            return i + 1;
        }

        /// <summary>
        /// Sorts a serialized array/list using the elements' CompareTo function (IComparable)
        /// </summary>
        /// <param name="arr">The serialized array with elements not inheriting ScriptableObjects</param>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        public static void SortSerializedPropertyArray(SerializedProperty arr, int start, int end)
        {
            int[] stack = new int[end - start + 1];
            int top = -1;
            stack[++top] = start;
            stack[++top] = end;

            while (top >= 0)
            {
                end = stack[top--];
                start = stack[top--];
                int p = PartitionSerializedArray(arr, start, end);

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