using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits.MVVM
{
    public abstract partial class ViewModelBase
    {
        private readonly ConcurrentDictionary<string, object>
            PropertyValueMapper = new ConcurrentDictionary<string, object>();

        public virtual object Identity
        {
            get => PropertyValueMapper[nameof(Identity)];
            set => PropertyValueMapper[nameof(Identity)] = value;
        }


        protected bool SetValue<TType>(ref TType field, TType newValue, [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
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
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }


        protected bool SetValue<TType>(TType newValue, [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (PropertyValueMapper.TryGetValue(propertyName, out object oldValue) && oldValue is TType old)
            {
                if (EqualityComparer<TType>.Default.Equals(old, newValue))
                {
                    return false;
                }
            }

            RaisePropertyChanging(propertyName);
            PropertyValueMapper[propertyName] = newValue;
            RaisePropertyChanged(propertyName); 
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }


        protected bool SetValue<TType>(TType newValue, IEqualityComparer<TType> comparer,
            [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (PropertyValueMapper.TryGetValue(propertyName, out object oldValue) && oldValue is TType old)
            {
                if (comparer.Equals(old, newValue))
                {
                    return false;
                }
            }

            RaisePropertyChanging(propertyName);
            PropertyValueMapper[propertyName] = newValue;
            RaisePropertyChanged(propertyName);
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }

        protected TType GetValue<TType>(TType defaultValue = default, [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (PropertyValueMapper.TryGetValue(propertyName, out object value))
            {
                return (TType)value;
            }

            PropertyValueMapper[propertyName] = defaultValue;
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return defaultValue;
        }
    }
}