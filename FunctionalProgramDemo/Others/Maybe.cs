using System;

namespace FunctionalProgramDemo
{
    public class Maybe
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;
        public Exception Exception {get;}

        protected Maybe(bool isSuccess, string error, Exception detail= null)
        {
            if (isSuccess && error != string.Empty)
                throw new InvalidOperationException();
            if (!isSuccess && error == string.Empty)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
            Exception = detail;
        }

        public static Maybe Fail(string message, Exception detail = null)
        {
            return new Maybe(false, message,detail);
        }

        public static Maybe<T> Fail<T>(string message, Exception detail = null)
        {
            return new Maybe<T>(default(T), false, message,detail);
        }

        public static Maybe Ok()
        {
            return new Maybe(true, string.Empty);
        }

        public static Maybe<T> Ok<T>(T value)
        {
            return new Maybe<T>(value, true, string.Empty);
        }
    }
    public class Maybe<T> : Maybe
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        protected internal Maybe(T value, bool isSuccess, string error, Exception detail = null)
            : base(isSuccess, error,detail)
        {
            _value = value;
        }


    }
}
