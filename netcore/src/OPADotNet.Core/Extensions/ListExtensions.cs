using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Core.Extensions
{
    internal static class ListExtensions
    {
        public static bool AreEqual<T>(this List<T> list, List<T> other)
        {
            if (list == null && other == null)
            {
                return true;
            }
            if (list == null || other == null)
            {
                return false;
            }
            if (list.Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (!Equals(list[i], other[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
