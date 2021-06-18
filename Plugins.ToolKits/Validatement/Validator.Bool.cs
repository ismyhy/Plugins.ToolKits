using Plugins.ToolKits.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Plugins.ToolKits.Validatement
{
    internal partial class Validator
    {
        private readonly IDictionary<PropertyInfo, List<Func<bool>>> _validateBoolAction =
            new ConcurrentDictionary<PropertyInfo, List<Func<bool>>>();
        public void Register<TReturnType>(Expression<Func<TReturnType>> expression,
            Func<TReturnType, bool> validatorFunc, string validateErrorMessage)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string propertyName = expression.GetMemberName();

            Register(propertyName, validatorFunc, validateErrorMessage);
        }

        public void Register<TReturnType>(string propertyName, Func<TReturnType, bool> validatorFunc,
            string validateErrorMessage)
        {
            if (validatorFunc is null)
            {
                throw new ArgumentNullException(nameof(validatorFunc));
            }

            if (validateErrorMessage is null)
            {
                throw new ArgumentNullException(nameof(validateErrorMessage));
            }

            PropertyInfo propertyInfo = _ownerType.GetProperty(propertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"can not find PropertyInfo:{propertyName} in Type:{_ownerType}");
            }

            InnerRegister(propertyInfo, validateErrorMessage, () =>
            {
                TReturnType value = (TReturnType)propertyInfo.GetValue(_owner);
                bool result = validatorFunc(value);
                return result;
            });
        }

        public void Register<TReturnType>(Expression<Func<TReturnType>> expression, Func<bool> validatorFunc,
            string validateErrorMessage)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string propertyName = expression.GetMemberName();

            Register(propertyName, validatorFunc, validateErrorMessage);
        }

        public void Register(string propertyName, Func<bool> validatorFunc, string validateErrorMessage)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (validatorFunc is null)
            {
                throw new ArgumentNullException(nameof(validatorFunc));
            }

            if (validateErrorMessage is null)
            {
                throw new ArgumentNullException(nameof(validateErrorMessage));
            }

            PropertyInfo propertyInfo = _ownerType.GetProperty(propertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"can not find PropertyInfo:{propertyName} in Type:{_ownerType}");
            }

            InnerRegister(propertyInfo, validateErrorMessage, validatorFunc);
        }


        private void InnerRegister(PropertyInfo propertyInfo, string validateErrorMessage, Func<bool> validatorFunc)
        {
            if (!_validateBoolAction.TryGetValue(propertyInfo, out List<Func<bool>> actions))
            {
                _validateBoolAction[propertyInfo] = actions = new List<Func<bool>>();
            }


            actions.Add(() =>
            {
                bool result = validatorFunc();

                if (!result)
                {
                    return false;
                }

                if (!_validateResult.TryGetValue(propertyInfo, out List<string> errors))
                {
                    _validateResult[propertyInfo] = errors = new List<string>();
                }

                errors.Add(validateErrorMessage);

                return true;
            });
        }
    }
}