using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Plugins.ToolKits
{
    public static class ReflectionExtensions
    {
        public static string GetPropertyName<TSource>(Expression<Func<TSource, object>> keySelector)
        {
            if (keySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            UnaryExpression unaryExpression = keySelector.Body as UnaryExpression;

            if (unaryExpression?.Operand is MemberExpression memberExpression2)
            {
                return memberExpression2.Member.Name;
            }

            return string.Empty;
        }

        public static string GetPropertyName<TSource, TPropertyType>(
            this Expression<Func<TSource, TPropertyType>> keySelector)
        {
            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (keySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            UnaryExpression unaryExpression = keySelector.Body as UnaryExpression;

            if (unaryExpression?.Operand is MemberExpression memberExpression2)
            {
                return memberExpression2.Member.Name;
            }

            return string.Empty;
        }


        public static string GetMemberName<T>(this Expression<Func<T>> expression, bool compound = false)
        {
            Expression body = expression.Body;
            return GetMemberName(body, compound);
        }


        public static string GetMemberName(Expression expression, bool compound = false)
        {
            if (expression is MemberExpression memberExpression)
            {
                if (compound && memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    return GetMemberName(memberExpression.Expression) + "." + memberExpression.Member.Name;
                }

                return memberExpression.Member.Name;
            }

            if (!(expression is UnaryExpression unaryExpression))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Could not determine member from {0}", expression));
            }

            if (unaryExpression.NodeType != ExpressionType.Convert)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Cannot interpret member from {0}", expression));
            }

            return GetMemberName(unaryExpression.Operand);
        }


        public static T TryCast<T>(this object value)
        {
            if (value is null)
            {
                return default;
            }

            return typeof(T).IsValueType
                ? (T)Convert.ChangeType(value, typeof(T))
                : value is T typeValue
                    ? typeValue
                    : default;
        }
    }
}