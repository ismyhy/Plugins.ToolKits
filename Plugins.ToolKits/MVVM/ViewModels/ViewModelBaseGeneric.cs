using Plugins.ToolKits.Attributes;
using Plugins.ToolKits.Extensions;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Plugins.ToolKits.MVVM
{

    public abstract class ViewModelBase<TViewModel, TModel> : ViewModelBase<TViewModel>
        where TViewModel : ViewModelBase, new()
        where TModel : new()
    {
        private static readonly Lazy<TModel> Lazy = new Lazy<TModel>(LazyThreadSafetyMode.ExecutionAndPublication);
        public static TModel Model => Lazy.Value;
    }


    public abstract class ViewModelBase<TViewModel> : ViewModelBase where TViewModel : ViewModelBase, new()
    {
        private static readonly Lazy<TViewModel> Lazy = new Lazy<TViewModel>(LazyThreadSafetyMode.ExecutionAndPublication);
        public static TViewModel Instance => Lazy.Value;


        public virtual void RaisePropertyChanged([NotNull] Expression<Func<TViewModel, object>> propertyExpression)
        {
            if (propertyExpression is null)
            {
                throw new ArgumentException($"{nameof(propertyExpression)} is Null");
            }

            string propertyName = ReflectionExtensions.GetPropertyName(propertyExpression);

            RaisePropertyChanged(propertyName);
        }

        protected virtual void RaisePropertyListChanged(params Expression<Func<TViewModel, object>>[] propertyExpressions)
        {
            if (propertyExpressions == null || propertyExpressions.Length == 0)
            {
                return;
            }

            string[] list = propertyExpressions.Where(i => i != null).Select(ReflectionExtensions.GetPropertyName).ToArray();

            RaisePropertyListChanged(list);
        }

    }
}