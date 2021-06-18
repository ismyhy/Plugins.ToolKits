using Plugins.ToolKits.Attributes;

using System;
using System.Linq;
using System.Reflection;

namespace Plugins.ToolKits.Extensions
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


    }
}