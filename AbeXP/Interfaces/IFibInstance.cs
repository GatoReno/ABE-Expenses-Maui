using Firebase.Database;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Interfaces
{
    public interface IFibInstance
    {
        FirebaseClient GetInstance();
    }
}
