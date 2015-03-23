using System;
using System.Collections.Generic;

namespace Match3Core
{
    static class ListExtensions
    {
        // Shuffle array (Fisher–Yates shuffle algorithm)
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.Swap(i, random.Next(i, list.Count));
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static T GetRandomElement<T>(this IList<T> list, Random random)
        {
            return list[random.Next(list.Count)];
        }
    }
}
