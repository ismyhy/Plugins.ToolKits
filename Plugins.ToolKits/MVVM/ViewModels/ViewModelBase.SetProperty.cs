using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits.MVVM
{
    public abstract partial class ViewModelBase
    {
        protected bool SetProperty<TType>(ref TType field, TType newValue, [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
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

        protected bool SetProperty<TType>(ref TType field, TType newValue, IEqualityComparer<TType> comparer,
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

            if (comparer.Equals(field, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            field = newValue;
            RaisePropertyChanged(propertyName);
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }

        protected bool SetProperty<TType>(TType oldValue, TType newValue, Action<TType> callback,
            [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (EqualityComparer<TType>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            callback(newValue);
            RaisePropertyChanged(propertyName);
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }

        protected bool SetProperty<TType>(TType oldValue, TType newValue, IEqualityComparer<TType> comparer, Action<TType> callback, [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (comparer.Equals(oldValue, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            callback(newValue);
            RaisePropertyChanged(propertyName);
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }


        protected bool SetProperty<TModel, TType>(TType oldValue, TType newValue, TModel model,
            Action<TModel, TType> callback,
            [CallerMemberName] string propertyName = null, params string[] affectOtherPropertyNames)
            where TModel : class
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (EqualityComparer<TType>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            callback(model, newValue);
            RaisePropertyChanged(propertyName);
            RaisePropertyListChangedAsync(affectOtherPropertyNames);
            return true;
        }
    }
}
