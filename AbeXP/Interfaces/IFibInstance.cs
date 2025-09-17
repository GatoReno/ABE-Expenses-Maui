using Firebase.Database;

namespace AbeXP.Interfaces
{
    public interface IFibInstance
    {
        FirebaseClient GetInstance();
    }
}
