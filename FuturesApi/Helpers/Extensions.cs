namespace FuturesApi.Helpers
{
    using System.Collections.Generic;

    public static class Extensions
    {
        public static bool IsListOk<T>(this List<T> elements)
        {
            if (elements == null || elements.Count == 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsArrayOk<T>(this T[] elements)
        {
            if (elements == null || elements.Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}