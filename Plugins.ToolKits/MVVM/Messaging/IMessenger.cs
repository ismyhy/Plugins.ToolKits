
using System;
using System.Threading.Tasks;

namespace Plugins.ToolKits.MVVM
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RecipientMethodTokenAttribute : Attribute
    {
        public RecipientMethodTokenAttribute(object token)
        {
            Token = token ?? new ArgumentNullException(nameof(token));
        }

        internal object Token { get; }
    }

    public interface IMessenger
    {
        void UnregisterAll(object recipient);

        void Unregister(object recipient, object token);

        void Send(object token, params object[] messageParams);

        Task SendAsync(object token, params object[] messageParams);

        TResult Send<TResult>(object token, params object[] messageParams);


        Task<TResult> SendAsync<TResult>(object token, params object[] messageParams);

        /// <summary>
        ///     <para>The object callback method must be labeled by  <see cref="RecipientMethodTokenAttribute" /></para>
        /// </summary>
        /// <typeparam name="TRecipient">Recipient Object</typeparam>
        /// <param name="recipient"></param>
        /// <returns></returns>
        bool Register<TRecipient>([NotNull] TRecipient recipient, RegistrationMode registrationMode = RegistrationMode.Once)
            where TRecipient : class;

        #region 0

        bool Register<TResult>(object recipient, object token, Func<TResult> callbackHandler);

        bool Register(object recipient, object token, Action callbackHandler,
            RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 1

        bool Register<TMessage, TResult>(object recipient, object token, Func<TMessage, TResult> callbackHandler);

        bool Register<TMessage>(object recipient, object token, Action<TMessage> callbackHandler,
            RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion

        #region 2

        bool Register<TMessage1, TMessage2, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2>(object recipient, object token,
            Action<TMessage1, TMessage2> callbackHandler, RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 3

        bool Register<TMessage1, TMessage2, TMessage3, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3> callbackHandler, RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 4

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4> callbackHandler,
            RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion

        #region 5

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5> callbackHandler,
            RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 6

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TResult>(object recipient,
            object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6> callbackHandler,
            RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 7

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TResult>(
            object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7>(object recipient,
            object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7> callbackHandler,
            RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 8

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TResult>(
            object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TResult>
                callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8>(
            object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8>
                callbackHandler, RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 9

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
            TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8,
            TMessage9>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9>
                callbackHandler, RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 10

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
            TMessage10, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
            TMessage10>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10> callbackHandler, RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion


        #region 11

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
            TMessage10, TMessage11, TResult>(object recipient, object token,
            Func<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10, TMessage11, TResult> callbackHandler);

        bool Register<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
            TMessage10, TMessage11>(object recipient, object token,
            Action<TMessage1, TMessage2, TMessage3, TMessage4, TMessage5, TMessage6, TMessage7, TMessage8, TMessage9,
                TMessage10, TMessage11> callbackHandler, RegistrationMode registrationMode = RegistrationMode.Once);

        #endregion
    }
}