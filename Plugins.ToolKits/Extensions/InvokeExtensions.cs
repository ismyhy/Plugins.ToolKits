﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Extensions
{
    public static class Invoker
    {
        public static void RunIgnore<TException>(Action action, Action<TException> exceptionCallback = null)
         where TException : Exception
        {
            if (action is null)
            {
                return;
            }

            try
            {
                action();
            }
            catch (TException exception)
            {
                exceptionCallback?.Invoke(exception);
            }
        }

        public static void For(int startIndex, int endIndex, Action<int> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                action(i);
            }

        }


        public static void For(int startIndex, int endIndex, Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                action();
            }

        }


        public static Task ForAsync(int startIndex, int endIndex, Action<int> action, CancellationToken token = default)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            CancellationToken token2 = token == default ? CancellationToken.None : token;
            return Task.Factory.StartNew(() =>
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    action(i);
                }
            }, token2, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

        }


        public static Task ForAsync(int startIndex, int endIndex, Action action, CancellationToken token = default)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            CancellationToken token2 = token == default ? CancellationToken.None : token;
            return Task.Factory.StartNew(() =>
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    action();
                }
            }, token2, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

        }
        public static Task WhileAsync(Func<bool> loopCondition, Action action, CancellationToken token = default)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (loopCondition is null)
            {
                throw new ArgumentNullException(nameof(loopCondition));
            }
            CancellationToken token2 = token == default ? CancellationToken.None : token;
            return Task.Factory.StartNew(() =>
            {
                while (loopCondition.Invoke())
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    action();
                }
            }, token2, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);


        }

        public static void While(Func<bool> loopCondition, Action action)
        {
            if (action is null || loopCondition is null)
            {
                return;
            }

            while (loopCondition.Invoke())
            {
                action();
            }
        }

        public static Task RunAsync(Action action, CancellationToken token = default, TaskCreationOptions creationOptions = TaskCreationOptions.DenyChildAttach)
        {
            if (action is null)
            {
                return Task.Delay(0);
            }

            CancellationToken token2 = token == default ? CancellationToken.None : token;

            return Task.Factory.StartNew(action, token2, creationOptions, TaskScheduler.Default);
        }


        public static Task<TResult> RunAsync<TResult>(Func<TResult> action, CancellationToken token = default, TaskCreationOptions creationOptions = TaskCreationOptions.DenyChildAttach)
        {
            if (action is null)
            {
                return Task.FromResult<TResult>(default);
            }

            CancellationToken token2 = token == default ? CancellationToken.None : token;

            return Task.Factory.StartNew(action, token2, creationOptions, TaskScheduler.Default);
        }

    }
}