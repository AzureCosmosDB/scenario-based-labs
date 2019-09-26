using System;
using System.Collections.Generic;
using System.Linq;

namespace Contoso.Apps.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Extension method that extends IEnumerable collections by
        /// giving a random (hopefully unique!) number to each element,
        /// then ordering the elements according to that number.
        /// <example>
        /// If you need to pull out three cards from a deck of 52, you
        /// can call deck.Shuffle().Take(3) and only three swaps will
        /// take place (although the entire array would have to be
        /// copied first).
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var rng = new Random();

            T[] elements = source.ToArray();

            // Note i > 0 to avoid final pointless iteration.
            for (int i = elements.Length - 1; i > 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                int swapIndex = rng.Next(i + 1);

                yield return elements[swapIndex];

                elements[swapIndex] = elements[i];
                // We don't actually perform the swap, we can forget about the
                // swapped element because we already returned it.
            }

            // There is one item remaining that was not returned - we return it now.
            yield return elements[0];
        }
    }
}