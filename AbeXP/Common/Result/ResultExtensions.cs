using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Common.Result
{
    public static class ResultExtensions
    {
        public static void Merge(this Result result, params Result[] results)
        {
            var errors = results.SelectMany(r => r.Errors).ToList();
            result.AddRange(errors);

            var warnings = results.SelectMany(r => r.Warnings).ToList();
            result.AddRange(warnings);
        }

        public static void MergeErrorsAsWarnings(this Result result, params Result[] results)
        {
            var errors = results.SelectMany(r => r.Errors).ToList();
            result.AddRange(errors.Select(x => new Warning(x.Message)).ToList());

            var warnings = results.SelectMany(r => r.Warnings).ToList();
            result.AddRange(warnings);
        }

        public static Result<T> ToResult<T>(this Result result) where T : notnull
        {
            var resultT = Result<T>.Fail([.. result.Errors]);
            resultT.AddRange(result.Warnings);

            return resultT;
        }
    }
}
