using Microsoft.EntityFrameworkCore;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
using NetCore.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data.Entity;

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
            user = _dbFirstDbContext.Users.FromSqlRaw<User>("SELECT * FROM dbo.[User]")
                            .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
                           .FirstOrDefault();

            // View
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>("SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.[uvwUser]")
            //                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //               .FirstOrDefault();

            // FUNCTION
            //user = _dbFirstDbContext.Users.FromSqlInterpolated<User>($"SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.ufnUser('{userId}','{password}')")
            //                .FirstOrDefault();

            // STORED PROCEDURE
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password })
            //    .AsEnumerable().ToList().FirstOrDefault();
            if(user == null)
            {
                // 접속 실패 횟수에 대한 증가
                int rowAffected;
                // SQL 문 직접 작성
                //rowAffected = _dbFirstDbContext.Database.ExecuteSqlInterpolated($"Update dbo.[User] Set AccessFailedCount +=1 WHERE UserId={userId}");
                // STORED PROCEDURE
                rowAffected = _dbFirstDbContext.Database.ExecuteSqlRaw($"dbo.FailedLoginByUserId @p0",parameters: new[] {userId});

            }

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
        private User GetUserInfo(string userid)
        {
            return _dbFirstDbContext.Users.Where(u => u.UserId.Equals(userid)).FirstOrDefault();
        }
        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _dbFirstDbContext.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();
            foreach (var role in userRolesByUserInfos)
            {
                role.UserRole = GetUserRole(role.RoleId);
            }
            return userRolesByUserInfos.OrderByDescending(uru => uru.UserRole.RolePriority);
        }
        private UserRole GetUserRole(string roleId)
        {
            var temp = _dbFirstDbContext.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
            return temp;
        }
        User IUser.GetUserInfo(string userid)
        {
            return GetUserInfo(userid);
        }

        public IEnumerable<UserRolesByUser> GetRolesOwneByUser(string userid)
        {
            return GetUserRolesByUserInfos(userid);
        }
        #endregion
    }
}
