using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Common.Result
{
    public abstract record ResultBaseModel(string Message);

    public record Error(string Message) : ResultBaseModel(Message);

    public record Warning(string Message) : ResultBaseModel(Message);
}
