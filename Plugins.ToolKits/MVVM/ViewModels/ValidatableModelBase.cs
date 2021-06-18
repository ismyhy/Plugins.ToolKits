using Plugins.ToolKits.Validatement;

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits.MVVM
{
    public abstract class ValidatableViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        internal readonly Validator Validator;

        protected ValidatableViewModelBase()
        {
            Validator = new Validator(this);
            ValidateItems(Validator);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return Validator.GetErrors(propertyName);
        }


        public bool HasErrors => Validator.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IErrorCollection GetErrors()
        {
            return Validator.GetErrors();
        }

        public void Validate()
        {
            Validator.Validate();
        }

        protected abstract void ValidateItems(IValidate validator);


        protected virtual void ValidatePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            Validator.Validate(propertyName);
        }
    }
}