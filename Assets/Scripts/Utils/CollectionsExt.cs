using System.Collections.Generic;

namespace Utils
{
    public static class CollectionsExt
    {
        public static List<T> ListOf<T>(params T[] items)
        {
            return new List<T>(items);
        }
    }
}