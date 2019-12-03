using System;
using System.Collections.Generic;
using System.Text;

namespace Hataraku.Bot.Entities.Extensions
{
    public static class StringExtensions
    {
        public static int GetLevenshteinDistance(this string src, string other)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(other)) return 0;

            int lengthA = src.Length;
            int lengthB = other.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = other[j - 1] == src[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }
    }
}
