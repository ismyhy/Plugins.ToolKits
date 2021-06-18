using System;

namespace Plugins.ToolKits.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool TryGet<TException>(this Exception exception, out TException outException)
        {
            outException = default;
            if (exception is null)
            {
                return false;
            }

            Exception ex = exception;

            if (ex is TException exx)
            {
                outException = exx;
                return true;
            }


            while (ex.InnerException is Exception eee)
            {
                if (eee is TException e)
                {
                    outException = e;
                    return true;
                }

                ex = eee;
            }

            return false;
        }
    }
}