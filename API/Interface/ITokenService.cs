

using API.Entities;

namespace API.Interface
{
    public interface ITokenService
    {
        //any class using/implementing this in our application should return string, and it has to take appuser as an argument
        //Its like a contract between interface and our implementation
        string CreateToken(AppUser user);
    }
}