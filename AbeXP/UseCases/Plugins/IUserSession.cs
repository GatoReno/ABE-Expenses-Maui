using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases.Plugins
{
    public interface IUserSession
    {
        string UserId {  get; }
        bool IsLoggedIn {  get; }
    }
}
