using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Bridges;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly DBFirstDbContext _context;

        public PasswordHasher(DBFirstDbContext dBFirstDbContext)
        {
            _context = dBFirstDbContext;
        }
        #region private Methods
        private string GetGUIDSalt()
        {
            return Guid.NewGuid().ToString();
        }
        private string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
        // UserId, Password 대소문자 처리
        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
             password: userId.ToLower() + password.ToLower() + guidSalt,
             salt: Encoding.UTF8.GetBytes(rngSalt),
             prf: KeyDerivationPrf.HMACSHA512,
             iterationCount: 45000, // 10000, 25000, 45000
             numBytesRequested: 256 / 8));
        }
        private bool CheckTheUserInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHash)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
        }
        private PasswodHashInfo SetPasswordInfo(string userId, string password)
        {
            string guidSalt = GetGUIDSalt();
            string rngSalt = GetRNGSalt();
            var passwodInfo = new PasswodHashInfo()
            {
                GUIDSalt = guidSalt,
                RNGSalt = rngSalt,
                PasswodHash = GetPasswordHash(userId, password, guidSalt, rngSalt)
            };
            return passwodInfo;
        }
        #endregion
        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return GetPasswordHash(userId,password, guidSalt, rngSalt);
        }

        bool IPasswordHasher.CheckTheUserInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHash)
        {
            return CheckTheUserInfo(userId,password,rngSalt,guidSalt,passwordHash);
        }

        PasswodHashInfo IPasswordHasher.SetPasswordInfo(string userId, string password)
        {
            return SetPasswordInfo(userId,password);
        }
    }
}
