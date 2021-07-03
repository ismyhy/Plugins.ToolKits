using System;
using System.Collections.Generic;
using System.Text;

namespace Plugins.ToolKits
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }


        public static bool IsNullOrWhiteSpace(this string stringValue)
        {
            return string.IsNullOrWhiteSpace(stringValue);
        }



        public static StringBuilder StringBuilder<T>(this IEnumerable<T> collection, Action<StringBuilder, T> action)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (T item in collection)
            {
                action(stringBuilder, item);
            }

            return stringBuilder;
        }

    }
}