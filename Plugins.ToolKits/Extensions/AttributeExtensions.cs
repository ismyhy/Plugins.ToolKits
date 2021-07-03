
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plugins.ToolKits
{
    public static partial class AttributeExtensions
    {
        public static TAttribute GetAttribute<TAttribute>([NotNull] this Type type) where TAttribute : Attribute
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            object obj = type.GetCustomAttributes(true).FirstOrDefault(i => i is TAttribute);

            return obj as TAttribute;
        }

        public static TAttribute GetAttribute<TAttribute>([NotNull] this PropertyInfo property) where TAttribute : Attribute
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            object obj = property.GetCustomAttributes(true).FirstOrDefault(i => i is TAttribute);

            return obj as TAttribute;
        }

        public static TAttribute GetAttribute<TAttribute>([NotNull] this FieldInfo field) where TAttribute : Attribute
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            object obj = field.GetCustomAttributes(true).FirstOrDefault(i => i is TAttribute);

            return obj as TAttribute;
        }

        public static TAttribute GetAttribute<TAttribute>([NotNull] this MemberInfo field) where TAttribute : Attribute
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            object obj = field.GetCustomAttributes(true).FirstOrDefault(i => i is TAttribute);

            return obj as TAttribute;
        }



        #region ENUM

        private static readonly ConcurrentDictionary<Type, IDictionary<Enum, Attribute[]>> enumAttributeDictionary = new ConcurrentDictionary<Type, IDictionary<Enum, Attribute[]>>();


        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            if (enumValue == null)
            {
                return default;
            }

            Type type = enumValue.GetType();

            if (!enumAttributeDictionary.TryGetValue(type, out IDictionary<Enum, Attribute[]> dicts))
            {
                enumAttributeDictionary[type] = dicts = new Dictionary<Enum, Attribute[]>();

                List<FieldInfo> list = enumValue.GetType().GetFields().Where(i => i.IsStatic && ! i.IsSpecialName).ToList();

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



        #endregion



    }
}