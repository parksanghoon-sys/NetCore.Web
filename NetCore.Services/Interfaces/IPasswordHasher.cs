using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string GetGUIDSalt();
        string GetRNGSalt();
        string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt);
        //bool MatchCheckTheUserInfo(string userId, string password);
        bool CheckTheUserInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHash)
    }
}
