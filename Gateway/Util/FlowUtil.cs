using System;

namespace Gateway.Util
{
    public class FlowUtil
    {
        public static TResult? TryCatchFinally<TResult, TException>(
            Func<TResult> function, Func<TException, TResult?> onError, Action doFinally)
            where TException : Exception
        {
            try { return function(); }
            catch (TException e) { return onError(e); }
            finally { doFinally(); }
        }

        public static TResult? TryCatchFinally<TResult>(
            Func<TResult> function, Func<Exception, TResult?> onError, Action doFinally)
            => TryCatchFinally<TResult, Exception>(function, onError, doFinally);

        public static TResult? TryCatch<TResult, TException>(
            Func<TResult> function, Func<TException, TResult?> onError)
            where TException : Exception
            => TryCatchFinally(function, onError, () => { });

        public static TResult? TryCatch<TResult>(
            Func<TResult> function, Func<Exception, TResult?> onError)
            => TryCatchFinally<TResult, Exception>(function, onError, () => { });
    }
}
