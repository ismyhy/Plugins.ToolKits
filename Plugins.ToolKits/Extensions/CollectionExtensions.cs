using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Plugins.ToolKits
{
    public static class CollectionExtensions
    {
        public static bool Exists<TType>(this IEnumerable<TType> origin, Func<TType, bool> matchAction)
        {
            return origin != null && origin.Any(matchAction);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> origin, Action<T> action)
        {
            if (origin == null || action == null)
            {
                return origin;
            }

            using IEnumerator<T> enumerator = origin.GetEnumerator();

            while (enumerator.MoveNext())
            {
                action.Invoke(enumerator.Current);
            }

            return origin;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> origin, int startIndex, Action<T> action)
        {
            if (origin == null || action == null)
            {
                return origin;
            }

            int num = startIndex;
            if (startIndex < 0 || startIndex >= origin.Count())
            {
                throw new IndexOutOfRangeException($"{nameof(startIndex)}:{startIndex}");
            }


            foreach (T arg in origin.Skip(startIndex))
            {
                action.Invoke(arg);
                num++;
            }

            return origin;
        }


        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> origin, int startIndex, Action<T, int> action)
        {

            if (origin == null || action == null)
            {
                return origin;
            }

            int num = startIndex;
            if (startIndex < 0 || startIndex >= origin.Count())
            {
                throw new IndexOutOfRangeException($"{nameof(startIndex)}:{startIndex}");
            }


            foreach (T arg in origin.Skip(startIndex))
            {
                action.Invoke(arg, num);
                num++;
            }

            return origin;
        }


        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> origin)
        {
            ObservableCollection<T> observableCollection = new ObservableCollection<T>();

            if (origin == null)
            {
                return observableCollection;
            }

            foreach (T item in origin)
            {
                observableCollection.Add(item);
            }

            return observableCollection;
        }

        public static ICollection<T> AddItems<T>(this ICollection<T> origin, IEnumerable<T> target)
        {
            if (target is null)
            {
                return origin;
            }


            origin ??= new List<T>();

            foreach (T item in target)
            {
                origin.Add(item);
            }

            return origin;
        }

        public static ICollection<T> AddItems<T>(this ICollection<T> origin, params T[] @params)
        {
            if ((@params?.Length ?? 0) == 0)
            {
                return origin;
            }


            origin ??= new List<T>();


            foreach (T item in @params)
            {
                origin.Add(item);
            }

            return origin;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is null)
            {
                return true;
            }

            return !enumerable.Any();
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


        public static bool Equal<T>(this IList collection1, IList collection2)
        {
            if (collection1 == null || collection2 == null)
            {
                return false;
            }

            if (collection1.Count != collection2.Count)
            {
                return false;
            }

            for (int i = 0, j = collection1.Count; i < j; i++)
            {
                if (!Equals(collection1[i], collection2[i]))
                {
                    return false;
                }
            }

            return true;
        }


        public static ICollection<TConcat> Concat<TConcat>(TConcat concat, params TConcat[] concats)
        {
            if (concats is null)
            {
                throw new ArgumentNullException(nameof(concats), $"{nameof(concats)} is Null");
            }



            List<TConcat> list = new List<TConcat>
            {
                concat
            };
            list.AddRange(concats);
            return list;
        }


        //public static IDictionary<TKey, TValue> With<TKey, TValue>(TKey key, TValue value)
        //{
        //    if (key is null)
        //    {
        //        throw new ArgumentNullException(nameof(key), $"{nameof(key)} is Null");
        //    }

        //    Dictionary<TKey, TValue> kv = new Dictionary<TKey, TValue>
        //    {
        //        [key] = value
        //    };
        //    return kv;
        //}


        //public static IDictionary<TKey, TValue> Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        //    TValue value)
        //{
        //    if (dictionary is null)
        //    {
        //        throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is Null");
        //    }

        //    if (key is null)
        //    {
        //        throw new ArgumentNullException(nameof(key), $"{nameof(key)} is Null");
        //    }

        //    dictionary[key] = value;

        //    return dictionary;
        //}


        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> funcValue)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is Null");
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} is Null");
            }

            if (funcValue is null)
            {
                throw new ArgumentNullException(nameof(funcValue), $"{nameof(funcValue)} is Null");
            }

            if (!dictionary.TryGetValue(key, out TValue value))
            {
                dictionary[key] = value = funcValue.Invoke();
            }


            return value;
        }


        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is Null");
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} is Null");
            }

            if (dictionary.Count == 0)
            {
                return false;
            }
            if (dictionary is ConcurrentDictionary<TKey, TValue> c)
            {
                return c.TryRemove(key, out TValue _);
            }

            if (!dictionary.TryGetValue(key, out _))
            {
                return false;
            }

            dictionary.Remove(key);
            return true;
        }
    }
}