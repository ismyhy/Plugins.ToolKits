using System;
using System.Collections.Generic;

namespace Plugins.ToolKits.EventKits
{
    public class EasyEventHandle : IDisposable, IEquatable<object>, IEqualityComparer<EasyEventHandle>
    {
        private EasyEventInvoker _easyEventInvoker;

        internal EasyEventHandle(EasyEventInvoker easyEventInvoker)
        {
            EventToken = easyEventInvoker.EventToken;
            _easyEventInvoker = easyEventInvoker;
            Token = Guid.NewGuid();
        }

        private Guid Token { get; }

        public object EventToken { get; }

        public void Dispose()
        {
            _easyEventInvoker.Dispose();
            _easyEventInvoker = null;
        }

        public bool Equals(EasyEventHandle x, EasyEventHandle y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(EasyEventHandle obj)
        {
            return obj.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EasyEventHandle token))
            {
                return false;
            }

            return GetHashCode() == token.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }


        public static bool operator ==(EasyEventHandle token1, EasyEventHandle token2)
        {
            if (token1 is null || token2 is null)
            {
                return false;
            }

            return token1.GetHashCode() == token2.GetHashCode();
        }

        public static bool operator !=(EasyEventHandle token1, EasyEventHandle token2)
        {
            if (token1 is null || token2 is null)
            {
                return false;
            }

            return token1.GetHashCode() != token2.GetHashCode();
        }


        public override string ToString()
        {
            return EventToken?.ToString() ?? $"{Environment.TickCount}";
        }
    }
}