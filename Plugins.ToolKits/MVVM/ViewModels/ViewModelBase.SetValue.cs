using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits.MVVM
{
    public abstract partial class ViewModelBase
    {
        private readonly ConcurrentDictionary<string, object>
            _valueObjects = new ConcurrentDictionary<string, object>();

        private object _identity;

        public virtual object Identity
        {
            get => _identity;
            set => SetProperty(ref _identity, value, EqualityComparer<object>.Default);
        }


        protected bool SetValue<TType>(ref TType field, TType newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<TType>.Default.Equals(field, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }


        protected bool SetValue<TType>(TType newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (_valueObjects.TryGetValue(propertyName, out object oldValue) && oldValue is TType old)
            {
                if (EqualityComparer<TType>.Default.Equals(old, newValue))
                {
                    return false;
                }
            }

            RaisePropertyChanging(propertyName);
            _valueObjects[propertyName] = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }


        protected bool SetValue<TType>(TType newValue, IEqualityComparer<TType> comparer,
            [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (_valueObjects.TryGetValue(propertyName, out object oldValue) && oldValue is TType old)
            {
                if (comparer.Equals(old, newValue))
                {
                    return false;
                }
            }

            RaisePropertyChanging(propertyName);
            _valueObjects[propertyName] = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected TType GetValue<TType>(TType defaultValue = default, [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (_valueObjects.TryGetValue(propertyName, out object value))
            {
                return (TType)value;
            }

            _valueObjects[propertyName] = defaultValue;

            return defaultValue;
        }
    }
}