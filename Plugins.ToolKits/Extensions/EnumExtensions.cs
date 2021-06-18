using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plugins.ToolKits.Extensions
{
    public static partial class EnumExtensions
    {

        private static readonly IDictionary<Type, IDictionary<Enum, Attribute[]>> enumsDictionary = new ConcurrentDictionary<Type, IDictionary<Enum, Attribute[]>>();


        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            if (enumValue == null)
            {
                return default;
            }

            Type type = enumValue.GetType();

            if (!enumsDictionary.TryGetValue(type, out IDictionary<Enum, Attribute[]> dicts))
            {
                enumsDictionary[type] = dicts = new Dictionary<Enum, Attribute[]>();

                List<FieldInfo> list = enumValue.GetType().GetFields().Where(i => i.IsStatic).ToList();

                foreach (FieldInfo fieldInfo in list)
                {
                    if (fieldInfo.GetValue(null) is Enum @enum)
                    {
                        dicts[@enum] = fieldInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
                    }
                }
            }

            if (!dicts.TryGetValue(enumValue, out Attribute[] atts))
            {
                return default;
            }

            return atts.OfType<T>().FirstOrDefault();

        }


        public static ICollection<T> GetAllValue<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>().ToList();
        }


        public static IDictionary<TEnum, TAttribute> GetAttributes<TEnum, TAttribute>() where TAttribute : Attribute where TEnum : Enum
        {
            Type enumType = typeof(TEnum);

            if (!enumsDictionary.TryGetValue(enumType, out IDictionary<Enum, Attribute[]> dicts))
            {
                enumsDictionary[enumType] = dicts = new Dictionary<Enum, Attribute[]>();

                List<FieldInfo> list = enumType.GetFields().Where(i => i.IsStatic).ToList();

                foreach (FieldInfo fieldInfo in list)
                {
                    if (fieldInfo.GetValue(null) is TEnum @enum)
                    {
                        dicts[@enum] = fieldInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
                    }
                }
            }

            return dicts.ToDictionary(i => (TEnum)i.Key, i => i.Value.OfType<TAttribute>().FirstOrDefault());
        }


        public static IDictionary<Enum, TAttribute> GetAttributes<TAttribute>(Type enumType) where TAttribute : Attribute
        {
            if (!enumsDictionary.TryGetValue(enumType, out IDictionary<Enum, Attribute[]> dicts))
            {
                enumsDictionary[enumType] = dicts = new Dictionary<Enum, Attribute[]>();

                List<FieldInfo> list = enumType.GetFields().Where(i => i.IsStatic).ToList();

                foreach (FieldInfo fieldInfo in list)
                {
                    if (fieldInfo.GetValue(null) is Enum @enum)
                    {
                        dicts[@enum] = fieldInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
                    }
                }
            }

            return dicts.ToDictionary(i => i.Key, i => i.Value.OfType<TAttribute>().FirstOrDefault());
        }
    }
}