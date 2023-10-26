using Microsoft.EntityFrameworkCore;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
using NetCore.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        private  DBFirstDbContext _dbFirstDbContext;
        private  CodeFirstDbContext _codeFirstDbContext;
        public UserService(CodeFirstDbContext codeFirstDbContext, DBFirstDbContext dBFirstDbContext)
        {
            _codeFirstDbContext = codeFirstDbContext;
            _dbFirstDbContext = dBFirstDbContext;
        }
        public bool MatchTheUserInfo(LoginInfo loginInfo)
        {
            return checkTheUserInfo(loginInfo.UserId, loginInfo.Password);
        }

        private User GetUserInfo(string userId, string password)
        {
            User user = null;
            // Lamda
            //user = _codeFirstDbContext.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            // FromSql
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>("SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.[User]")
            //                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //               .FirstOrDefault();

            // View
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>("SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.[uvwUser]")
            //                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //               .FirstOrDefault();

            // FUNCTION
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>($"SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.ufnUser('{userId}','{password}')")
            //                .FirstOrDefault();

            // STORED PROCEDURE
            user = _dbFirstDbContext.Users.FromSqlRaw<User>("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password })
                .AsEnumerable().ToList().FirstOrDefault();

            return user;
        }
        #region private methods
        private IEnumerable<User> GetUserInfos()
        {
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "jadejs",
            //        UserName ="박상훈",
            //        UserEmail = "power@naver.com",
            //        Password = "123456"
            //    }
            //};
            //return _codeFirstDbContext.Users.ToList();
            return _dbFirstDbContext.Users.ToList();
        }
        private bool checkTheUserInfo(string userid, string password)
        {
            //return GetUserInfos().Where(u => u.UserId!.Equals(userid) && u.Password.Equals(password)).Any();
            return GetUserInfo(userid, password) != null ? true : false;
        }
        #endregion
    }
}
