using System.Collections.Generic;

namespace Utils
{
    public static class CollectionsExt
    {
        public static List<T> ListOf<T>(params T[] items) => new(items);
    }
}