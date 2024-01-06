using NetCore.Services.Bridges;
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
        bool CheckTheUserInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHash);
        /// <summary>
        /// [사용자 가입] 비밀번호 정보 지정
        /// </summary>
        /// <param name="userId">아이디</param>
        /// <param name="password">비밀번호</param>
        /// <returns></returns>
        PasswodHashInfo SetPasswordInfo(string userId, string password);
    }
}
