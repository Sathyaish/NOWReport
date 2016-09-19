using System;
using System.Collections.Generic;
using System.Linq;

namespace Wintellect.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool ThereAreNone<T>(this IEnumerable<T> source)
        {
            // Inefficient implementation
            // return source.Count<T>() == 0;

            // efficient implementation
            if (source.IsNull()) return true;

            using (var enumerator = source.GetEnumerator())
            {
                return !enumerator.MoveNext();
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var t in source)
                action(t);
        }

        public static bool ThereAreLessThan<T>(this IEnumerable<T> source, int number)
        {
            // inefficient implementation
            // return source.Count<T>() < number;

            // efficient implementation
            source.ThrowIfNull("Argument null: source");

            int i = 0;

            foreach (var t in source)
            {
                if (++i == number) return false;
            }

            return true;
        }

        public static bool ThereAreMoreThan<T>(this IEnumerable<T> source, int number)
        {
            // inefficient implementation
            // return source.Count<T>() > number;

            // efficient implementation
            source.ThrowIfNull("Argument null: source");

            int i = 0;

            foreach (var t in source)
            {
                if (++i > number) return true;
            }

            return false;
        }

        public static bool ThereAreSome<T>(this IEnumerable<T> source)
        {
            return !source.ThereAreNone();
        }

        public static bool ThereAre<T>(this IEnumerable<T> source, int number)
        {
            // inefficient implementation
            // return source.Count<T>() == number;

            // efficient implementation
            source.ThrowIfNull("Argument null: source");

            int i = 0;
            using (var sourceEnumerator = source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    if (++i > number) return false;
                }
            }

            return i == number;
        }

        public static bool ThereAreNot<T>(this IEnumerable<T> source, int number)
        {
            return !source.ThereAre(number);
        }

        public static bool IsABunchOfGroups<T>(this IEnumerable<T> source)
        {
            return typeof(T).IsAssignableFrom(typeof(IGrouping<,>));
        }
    }
}
