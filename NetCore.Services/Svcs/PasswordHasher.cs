using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
             password: userId + password + guidSalt,
             salt: Encoding.UTF8.GetBytes(rngSalt),
             prf: KeyDerivationPrf.HMACSHA512,
             iterationCount: 45000, // 10000, 25000, 45000
             numBytesRequested: 256 / 8));
        }
        private bool MatchCheckTheUserInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHash)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
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

        bool IPasswordHasher.MatchCheckTheUserInfo(string userId, string password)
        {
            var user = _context.Users.Where(c => c.UserId.Equals(userId)).FirstOrDefault();

            string guidSalt = user.GUIDSalt;
            string rngSalt = user.RNGSalt;
            string passwordHash = user.PasswordHash;
            return MatchCheckTheUserInfo(userId, password,rngSalt, guidSalt, passwordHash);
        }
    }
}
