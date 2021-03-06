﻿
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.MVVM
{
    public abstract partial class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected CommandBinder CommandBinder { get; } = new CommandBinder();

        public virtual void NotifyPropertyChanged(params string[] propertyNames)
        {
            if(propertyNames is null|| propertyNames.Length == 0)
            {
                return;
            }

            RaisePropertyListChanged(propertyNames);
        }

        public virtual Task NotifyPropertyChangedAsync(params string[] propertyNames)
        {
            if (propertyNames is null || propertyNames.Length == 0)
            {
                return Task.Delay(0);
            }

            return RaisePropertyListChangedAsync(propertyNames);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }


        protected virtual void RaisePropertyListChanged(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                return;
            }

            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            foreach (string propertyName in propertyNames.Where(i => i != null).ToArray())
            {
                propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected virtual Task RaisePropertyListChangedAsync(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                return Task.Delay(0);
            }
            return Task.Factory.StartNew(() =>
            {
                PropertyChangedEventHandler propertyChanged = PropertyChanged;

                propertyNames.Where(i => i != null).ToArray().ForEach(propertyName =>
                {
                    propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }


        public virtual void Dispose()
        {
            PropertyValueMapper?.Clear();
        }
    }
}