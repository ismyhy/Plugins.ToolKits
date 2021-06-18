using System;
using System.Linq.Expressions;

namespace Plugins.ToolKits.Validatement
{
    public interface IValidate
    {
        void Register<TReturnType>(Expression<Func<TReturnType>> expression, Func<TReturnType, bool> validatorFunc,
            string validateErrorMessage);

        void Register<TReturnType>(string propertyName, Func<TReturnType, bool> validatorFunc,
            string validateErrorMessage);


        void Register<TReturnType>(Expression<Func<TReturnType>> expression, Func<bool> validatorFunc,
            string validateErrorMessage);

        void Register(string propertyName, Func<bool> validatorFunc,
            string validateErrorMessage);


        void Register<TReturnType>(Expression<Func<TReturnType>> expression,
            Func<TReturnType, ValidateResult> validatorFunc,
            string validateErrorMessage);

        void Register<TReturnType>(string propertyName, Func<TReturnType, ValidateResult> validatorFunc,
            string validateErrorMessage);


        void Register<TReturnType>(Expression<Func<TReturnType>> expression, Func<ValidateResult> validatorFunc,
            string validateErrorMessage);

        void Register(string propertyName, Func<ValidateResult> validatorFunc,
            string validateErrorMessage);


        void Required<TReturnType>(Expression<Func<TReturnType>> expression, string validateErrorMessage = null);

        //void MaxLength(Expression<Func<string>> expression, string validateErrorMessage = null);
    }
}