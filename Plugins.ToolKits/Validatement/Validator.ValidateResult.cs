
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Plugins.ToolKits.Validatement
{
    internal partial class Validator
    {
        private readonly IDictionary<PropertyInfo, List<Func<ValidateResult>>> _validateResultAction =
            new ConcurrentDictionary<PropertyInfo, List<Func<ValidateResult>>>();


        public void Register<TReturnType>(Expression<Func<TReturnType>> expression,
            Func<TReturnType, ValidateResult> validatorFunc, string validateErrorMessage)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string propertyName = expression.GetMemberName();

            ((IValidate)this).Register(propertyName, validatorFunc, validateErrorMessage);
        }

        public void Register<TReturnType>(string propertyName, Func<TReturnType, ValidateResult> validatorFunc,
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

            PropertyInfo propertyInfo = ownerType.GetProperty(propertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"can not find PropertyInfo:{propertyName} in Type:{ownerType}");
            }

            InnerResultRegister(propertyInfo, validateErrorMessage, () =>
            {
                TReturnType value = (TReturnType)propertyInfo.GetValue(owner);
                ValidateResult result = validatorFunc(value);
                return result;
            });
        }

        public void Register<TReturnType>(Expression<Func<TReturnType>> expression, Func<ValidateResult> validatorFunc,
            string validateErrorMessage)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string propertyName = expression.GetMemberName();

            ((IValidate)this).Register(propertyName, validatorFunc, validateErrorMessage);
        }

        public void Register(string propertyName, Func<ValidateResult> validatorFunc, string validateErrorMessage)
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

            PropertyInfo propertyInfo = ownerType.GetProperty(propertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"can not find PropertyInfo:{propertyName} in Type:{ownerType}");
            }

            InnerResultRegister(propertyInfo, validateErrorMessage, validatorFunc);
        }


        private void InnerResultRegister(PropertyInfo propertyInfo, string validateErrorMessage,
            Func<ValidateResult> validatorFunc)
        {
            if (!_validateResultAction.TryGetValue(propertyInfo, out List<Func<ValidateResult>> actions))
            {
                _validateResultAction[propertyInfo] = actions = new List<Func<ValidateResult>>();
            }


            actions.Add(() =>
            {
                ValidateResult result = validatorFunc();

                if (result == ValidateResult.Invalid)
                {
                    if (!validateResult.TryGetValue(propertyInfo, out List<string> errors))
                    {
                        validateResult[propertyInfo] = errors = new List<string>();
                    }

                    errors.Add(validateErrorMessage);
                    return ValidateResult.Invalid;
                }

                return ValidateResult.Valid;
            });
        }
    }
}