using Plugins.ToolKits.Extensions;
using Plugins.ToolKits.MVVM;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Plugins.ToolKits.Validatement
{
    internal partial class Validator : IValidate
    {
        private readonly ValidatableViewModelBase _owner;

        private readonly Type _ownerType;

        private readonly IDictionary<PropertyInfo, List<string>> _validateResult =
            new ConcurrentDictionary<PropertyInfo, List<string>>();

        internal Validator(ValidatableViewModelBase validateOwner)
        {
            _owner = validateOwner;
            _ownerType = validateOwner.GetType();
        }

        public bool HasErrors => _validateResult.Count > 0;


        public void Validate()
        {
            _validateResult.ForEach(i => i.Value?.Clear());
            _validateResult.Clear();

            _validateResultAction.ForEach(i =>
            {
                foreach (Func<ValidateResult> item in i.Value)
                {
                    if (item.Invoke() == ValidateResult.Invalid)
                    {
                        break;
                    }
                }
            });


            _validateBoolAction.ForEach(i =>
            {
                foreach (Func<bool> item in i.Value)
                {
                    if (item.Invoke())
                    {
                        break;
                    }
                }
            });

        }


        public void Validate(string propertyName, bool clearOld = true)
        {
            PropertyInfo propertyInfo = _ownerType.GetProperty(propertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"can not find PropertyInfo:{propertyName} in Type:{_ownerType}");
            }

            if (clearOld && _validateResult.TryGetValue(propertyInfo, out List<string> reList))
            {
                reList?.Clear();
            }

            if (_validateBoolAction.TryGetValue(propertyInfo, out List<Func<bool>> value))
            {
                value.ForEach(ii => ii?.Invoke());
            }
        }


        public IErrorCollection GetErrors()
        {

            Dictionary<string, List<string>> dict = _validateResult.GroupBy(i =>
            {
                DisplayNameAttribute dis = i.Key.GetAttribute<DisplayNameAttribute>();
                return dis is null ? i.Key.Name : dis.DisplayName;
            }).ToDictionary(grouping => grouping.Key, grouping => grouping.SelectMany(keyValue => keyValue.Value).ToList());

            ErrorCollection error = new ErrorCollection();

            if (dict.Count > 0)
            {
                foreach (KeyValuePair<string, List<string>> items in dict)
                {
                    error[items.Key] = items.Value;
                }
            }

            return error;
        }


        public ICollection<string> GetErrors(string propertyName)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyInfo propertyInfo = _validateResult.Keys.FirstOrDefault(i => i.Name == propertyName);
            if (propertyInfo is null)
            {
                return new List<string>();
                // throw new ArgumentException($"can not find PropertyInfo:{propertyName} in Type:{_owner.GetType()}");
            }

            return _validateResult[propertyInfo];
        }

        public void Required<TReturnType>(Expression<Func<TReturnType>> expression, string validateErrorMessage = null)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string propertyName = expression.GetMemberName();

            ((IValidate)this).Register<TReturnType>(expression, i => i is null, validateErrorMessage ?? $"{propertyName} is Null");
        }

        //public void MaxLength(Expression<Func<string>> expression, string validateErrorMessage = null)
        //{
        //    throw new NotImplementedException();
        //}
    }
}