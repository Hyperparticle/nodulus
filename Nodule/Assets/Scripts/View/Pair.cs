namespace Assets.Scripts.Utility
{
    /// <summary>
    /// Contains helper methods for dealing with pairs of items
    /// </summary>
    public static class Pair
    {
        /// <summary>
        /// Swaps the two items unconditionally
        /// </summary>
        public static void Swap<T>(ref T first, ref T second)
        {
            var temp = first;
            first = second;
            second = temp;
        }

        /// <summary>
        /// Swaps the two items if the condition is true
        /// </summary>
        public static void Swap<T>(ref T first, ref T second, bool condition)
        {
            if (condition) Swap(ref first, ref second);
        }
    }
}

