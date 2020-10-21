using System;
namespace FunctionalProgramDemo
{
    public static class MaybeExtension
    {
        public static Maybe<K> Railway<T, K>(this Maybe<T> result, Func<Maybe<T>, Maybe<K>> func)
        {
            if (result.IsFailure)
                return Maybe.Fail<K>(result.Error,result.Exception);
            return func(result);
        }
    }
}
