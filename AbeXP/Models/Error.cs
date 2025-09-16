using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class Error
    {
        public string Message { get; set; }
        public int? Code { get; set; }

        public Error(string message, int? code = null)
        {
            Message = message;
            Code = code;
        }

        public override string ToString() => Code.HasValue ? $"[{Code}] {Message}" : Message;

    }
}
