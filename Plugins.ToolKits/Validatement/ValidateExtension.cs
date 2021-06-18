using Plugins.ToolKits.MVVM;

using System;
using System.Linq.Expressions;

namespace Plugins.ToolKits.Validatement
{
    public static class ValidateExtension
    {
        public static void AddRequiredValidator<TReturnType>(this ValidatableViewModelBase validatableViewModel,
            Expression<Func<TReturnType>> expression, string validateErrorMessage = null)
        {
            string propertyName = expression.GetMemberName();

            ((IValidate)validatableViewModel.Validator).Register<TReturnType>(propertyName, i =>
           {
               if (i is null)
               {
                   return ValidateResult.Invalid;
               }

               return ValidateResult.Valid;
           }, validateErrorMessage ?? $"{propertyName} is NULL");
        }


        public static void AddMaxLengthValidator(this ValidatableViewModelBase validatableViewModel,
            Expression<Func<string>> expression, int maxLength, string validateErrorMessage = null)
        {
            string propertyName = expression.GetMemberName();
            ((IValidate)validatableViewModel.Validator).Register<string>(propertyName, i =>
               {
                   if (i is null)
                   {
                       return ValidateResult.Invalid;
                   }

                   if (i.Length > maxLength)
                   {
                       return ValidateResult.Invalid;
                   }

                   return ValidateResult.Valid;
               },
                validateErrorMessage ?? $"{propertyName} MaxLength is {maxLength}");
        }

        public static void AddMinLengthValidator(this ValidatableViewModelBase validatableViewModel,
            Expression<Func<string>> expression, int minLength, string validateErrorMessage = null)
        {
            string propertyName = expression.GetMemberName();
            ((IValidate)validatableViewModel.Validator).Register<string>(propertyName, i =>
               {
                   if (i is null)
                   {
                       return ValidateResult.Invalid;
                   }

                   if (i.Length < minLength)
                   {
                       return ValidateResult.Invalid;
                   }

                   return ValidateResult.Valid;
               },
                validateErrorMessage ?? $"{propertyName} MinLength is {minLength}");
        }
    }
}