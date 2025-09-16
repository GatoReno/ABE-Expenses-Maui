using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public List<Error> Errors { get; protected set; } = new();
        public Error? FirstError => Errors.FirstOrDefault();

        protected Result() { }

        public static Result Success() => new Result { IsSuccess = true };
        public static Result Failure(params Error[] errors) => new Result { IsSuccess = false, Errors = errors.ToList() };

        public static implicit operator Result(Error error) => Failure(error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; private set; }

        private Result() { }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static new Result<T> Failure(params Error[] errors) => new Result<T> { IsSuccess = false, Errors = errors.ToList() };

        // Implicit conversions
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure(error);
    }

}
