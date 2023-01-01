using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platform.Primitive
{
    public interface IResult
    {
        bool Succeed { get; set; }

        Guid TraceId { get; set; }
    }

    public interface IResult<TResult> : IResult
    {
        TResult Value { get; set; }
    }

    public class Result : IResult
    {
        public bool Succeed { get; set; }

        public Guid TraceId { get; set; }
    }

    public class Result<TValue> : Result, IResult<TValue>
    {
        public TValue Value { get; set; }

        public Result<TValue> UseResult(TValue value)
        {
            Value = value;
            return this;
        }

        public Result<TValue> Ok()
        {
            Succeed = true;
            return this;
        }

        public Result<TValue> Fail()
        {
            Succeed = false;
            return this;
        }

        public Result<TValue> Context(TraceContext context)
        {
            TraceId = context.TraceId;
            return this;
        }
    }

    public static class ResultExtensions
    {
        public static async Task<IEnumerable<T>> Extract<T>(this Task<Result<T>[]> task) =>
            (await task).Select(result => result.Value);
    }
}