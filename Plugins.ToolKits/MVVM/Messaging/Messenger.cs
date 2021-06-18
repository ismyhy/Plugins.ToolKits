using Plugins.ToolKits.Extensions;

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.MVVM
{
    public sealed class Messenger : IMessenger
    {
        private readonly Mapping<object, object, object> ActionMapping = new Mapping<object, object, object>();
        private readonly Mapping<object, object, object> FuncMapping = new Mapping<object, object, object>();


        public override string ToString()
        {
            return $"Action Count:{ActionMapping.Count()}  Func Count:{FuncMapping.Count()}";
        }


        public void UnregisterAll(object recipient)
        {
            ActionMapping.Remove(recipient);
            FuncMapping.Remove(recipient);
        }

        public void Unregister(object recipient, object token)
        {
            ActionMapping.Remove(token, recipient);
            FuncMapping.Remove(token, recipient);
        }

        public void Send(object token, params object[] messageParams)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (ActionMapping.Count(token) == 0)
            {
                throw new ArgumentException($"Unregistered token:{token}");
            }

            if (!ActionMapping.TryGets(token, out object[] recipients, out object[] handlers))
            {
                return;
            }

            for (int i = 0, j = recipients.Length; i < j; i++)
            {
                object handler = handlers[i];
                object recipient = recipients[i];
                if (handler is MethodInfo method)
                {
                    method.Invoke(recipient, messageParams);
                    continue;
                }

                MethodInfo methodInfo = handler.GetType().GetMethod("Invoke");
                methodInfo?.Invoke(handler, messageParams);
            }
        }

        public Task SendAsync(object token, params object[] messageParams)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            return Task.Factory.StartNew(() => Send(token, messageParams), CancellationToken.None,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public TResult Send<TResult>(object token, params object[] messageParams)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            if (FuncMapping.Count(token) == 0)
            {
                throw new ArgumentException($"Unregistered token:{token}");
            }

            if (!FuncMapping.TryGet(token, out object recipient, out object handler))
            {
                return default;
            }


            if (handler is MethodInfo method)
            {
                object result = method.Invoke(recipient, messageParams);
                return (TResult)result;
            }

            MethodInfo methodInfo = handler.GetType().GetMethod("Invoke");
            object result1 = methodInfo?.Invoke(handler, messageParams);
            return (TResult)result1;
        }

        public Task<TResult> SendAsync<TResult>(object token, params object[] messageParams)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            return Task.Factory.StartNew(() => Send<TResult>(token, messageParams), CancellationToken.None,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }



        public bool Register<TRecipient>(TRecipient recipient, RegistrationMode registrationMode = RegistrationMode.Once) where TRecipient : class
        {
            if (recipient is null)
            {
                return false;
            }

            BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
            System.Collections.Generic.Dictionary<MethodInfo, RecipientMethodTokenAttribute> dictionary = recipient.GetType().GetMethods(f)
                .ToDictionary(i => i, i => i.GetAttribute<RecipientMethodTokenAttribute>());
            System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<MethodInfo, RecipientMethodTokenAttribute>> methodInfos = dictionary.Where(i => i.Value != null).ToList();

            bool result = true;

            foreach (System.Collections.Generic.KeyValuePair<MethodInfo, RecipientMethodTokenAttribute> info in methodInfos)
            {
                result = result && TokenRegister(recipient, info.Value.Token, info.Key,
                    info.Key.ReturnType != typeof(void), registrationMode);
            }

            return result;
        }

        #region MyRegion

        private static IMessenger _defaultInstance;


        private static readonly object syncRootLock = new object();

        public static IMessenger Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (syncRootLock)
                    {
                        if (_defaultInstance == null)
                        {
                            lock (syncRootLock)
                            {
                                if (_defaultInstance == null)
                                {
                                    _defaultInstance = new Messenger();
                                }
                            }
                        }
                    }
                }

                return _defaultInstance;
            }
        }

        #endregion


        #region Register

        public bool Register<TResult>(object recipient, object token, Func<TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register(object recipient, object token, Action handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage, TResult>(object recipient, object token, Func<TMessage, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage>(object recipient, object token, Action<TMessage> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2>(object recipient, object token, Action<TMessage1, TMessage2> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TResult>(object recipient,
            object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TResult>(
            object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6>(object recipient,
            object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TResult>(
            object recipient,
            object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7>(
            object recipient,
            object token, Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TResult>(
            object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TResult>
                handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8>(
            object recipient,
            object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9,
            TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9>(
            object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9>
                handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9,
            TMessage10, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9,
            TMessage10>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9,
            TMessage10, TMessage11, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10, TMessage11, TResult> handler)
        {
            return TokenRegister(recipient, token, handler, true);
        }

        public bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9,
            TMessage10, TMessage11>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10, TMessage11> handler, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            return TokenRegister(recipient, token, handler, false, registrationMode);
        }


        private bool TokenRegister(object recipient, object token, object handler, bool hasReturnValue = false, RegistrationMode registrationMode = RegistrationMode.Once)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            if (recipient is null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }


            if (!hasReturnValue)
            {
                if (registrationMode == RegistrationMode.Once)
                {
                    if (ActionMapping.Count(token) > 0)
                    {
                        return false;
                    }
                }
                TokenComparator(token, handler);

                return ActionMapping.Add(token, recipient, handler);
            }


            if (registrationMode == RegistrationMode.More)
            {
                return false;
            }

            int count1 = FuncMapping.Count(token);

            if (count1 > 0)
            {
                throw new NotSupportedException($"token:{token} has been Registered");
            }

            return FuncMapping.Add(token, recipient, handler);
        }


        private void TokenComparator(object token, object handler)
        {
            static MethodInfo GetMethodInfo(object handlerValue)
            {
                if (handlerValue is MethodInfo methodInfo1)
                {
                    return methodInfo1;
                }

                PropertyInfo propertyInfo = handlerValue.GetType().GetProperty("Method");
                return propertyInfo?.GetValue(handlerValue) as MethodInfo;
            }

            MethodInfo methodInfo = GetMethodInfo(handler);
            if (methodInfo is null)
            {
                return;
            }

            Tuple<object, object, object> f = ActionMapping.FirstOrDefault(i => Equals(i.Item1, token));

            if (f is null)
            {
                return;
            }

            MethodInfo info = GetMethodInfo(f.Item3);
            if (info is null)
            {
                return;
            }
            ParameterInfo[] ps1 = methodInfo?.GetParameters();
            ParameterInfo[] ps2 = info?.GetParameters();

            if (ps1.Length != ps2.Length)
            {
                throw new NotSupportedException($"Token:{token} exists, but parameters do not match");
            }


            bool result = Enumerable.Range(0, ps2.Length).Aggregate(true, (s, e) =>
              {
                  int f1 = ps2[e].ParameterType.GetHashCode();
                  int f2 = ps1[e].ParameterType.GetHashCode();
                  return s && (f1 == f2);
              });

            if (!result)
            {
                throw new NotSupportedException($"Token:{token} exists, but parameters do not match");
            }
        }

        #endregion
    }
}