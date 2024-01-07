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
        //private  CodeFirstDbContext _codeFirstDbContext;
        private IPasswordHasher _passwordHasher;
        public UserService(CodeFirstDbContext codeFirstDbContext, DBFirstDbContext dBFirstDbContext, IPasswordHasher passwordHasher)
        {
            //_codeFirstDbContext = codeFirstDbContext;
            _dbFirstDbContext = dBFirstDbContext;
            _passwordHasher = passwordHasher;

        }
        
        private bool MatchTheUserInfo(LoginInfo loginInfo)
        {
            var user = _dbFirstDbContext.Users.Where(c => c.UserId.Equals(loginInfo.UserId)).FirstOrDefault();

            if(user == null)
                return false;

            return _passwordHasher.CheckTheUserInfo(loginInfo.UserId, loginInfo.Password, user.RNGSalt, user.GUIDSalt, user.PasswordHash);
            //return checkTheUserInfo(loginInfo.UserId, loginInfo.Password);
        }

        #region PrivateMethods
        // UserId 에 대해 대소문자 처리
        private int RegisterUser(RegisterInfo register)
        {
            var utcNow = DateTime.UtcNow;
            var passwordInfo = _passwordHasher.SetPasswordInfo(register.UserId!, register.Password!);
            var user = new User()
            {
                UserId = register.UserId!.ToLower(),
                UserName = register.UserName,
                UserEmail = register.UserEmail,
                GUIDSalt = passwordInfo.GUIDSalt,
                RNGSalt = passwordInfo.RNGSalt,
                PasswordHash = passwordInfo.PasswodHash,
                AccessFailedCount = 0,
                IsMembershipWithdrawn = false,
                JoinedUtcDate = utcNow
            };
            var userRolseByUser = new UserRolesByUser()
            {
                UserId = register.UserId.ToLower(),
                RoleId = "AssociateUser",
                OwnedUtcDate = utcNow
            };
            _dbFirstDbContext.Add(user);
            _dbFirstDbContext.Add(userRolseByUser);

            return _dbFirstDbContext.SaveChanges();
        }
        private UserInfo GetUserInfoForUpdate(string userId)
        {
            var user = GetUserInfo(userId);
            var userInfo = new UserInfo()
            {
                UserId = null,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                ChangeInfo = new ChangeInfo()
                {
                    UserEmail = user.UserEmail,
                    UserName = user.UserName
                }
            };
            return userInfo;
        }
        private int UpdateUser(UserInfo user)
        {                       
            var userInfo = _dbFirstDbContext.Users.Where(c => c.UserId.Equals(user.UserId)).FirstOrDefault();

            if(userInfo == null)
                return 0;

            bool check = _passwordHasher.CheckTheUserInfo(user.UserId, user.Password, userInfo.RNGSalt, userInfo.GUIDSalt, userInfo.PasswordHash);
            int rowAffected = 0;
            if (check)
            {                
                _dbFirstDbContext.Update(userInfo);
                userInfo.UserName = user.UserName;
                userInfo.UserEmail = user.UserEmail;
                rowAffected = _dbFirstDbContext.SaveChanges();
            }
            return rowAffected;
        }
        private bool CompareInfo(UserInfo userInfo)
        {
            return userInfo.ChangeInfo.Equals(userInfo);
        }
        private User GetUserInfo(string userId, string password)
        {
            User user = null;
            // Lamda
            //user = _codeFirstDbContext.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            // FromSql
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>("SELECT * FROM dbo.[User]")
            //                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //               .FirstOrDefault();

            // View
            //user = _dbFirstDbContext.Users.FromSqlRaw<User>("SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.[uvwUser]")
            //                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //               .FirstOrDefault();

            // FUNCTION
            //user = _dbFirstDbContext.Users.FromSqlInterpolated<User>($"SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.ufnUser('{userId}','{password}')")
            //                .FirstOrDefault();

            // STORED PROCEDURE
            user = _dbFirstDbContext.Users.FromSqlRaw<User>("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password })
                .AsEnumerable().ToList().FirstOrDefault();
            if (user == null)
            {
                // 접속 실패 횟수에 대한 증가
                int rowAffected;
                // SQL 문 직접 작성
                //rowAffected = _dbFirstDbContext.Database.ExecuteSqlInterpolated($"Update dbo.[User] Set AccessFailedCount +=1 WHERE UserId={userId}");
                // STORED PROCEDURE
                rowAffected = _dbFirstDbContext.Database.ExecuteSqlRaw($"dbo.FailedLoginByUserId @p0", parameters: new[] { userId });

            }

            return user;
        }

        private IEnumerable<User> GetUserInfos()
        {
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
        private int WithdrawnUser(WithdrawnInfo user)
        {
            var userInfo = _dbFirstDbContext.Users.Where(c => c.UserId.Equals(user.UserId)).FirstOrDefault();

            if (userInfo == null)
                return 0;

            bool check = _passwordHasher.CheckTheUserInfo(user.UserId, user.Password, userInfo.RNGSalt, userInfo.GUIDSalt, userInfo.PasswordHash);
            
            if (check)
            {
                _dbFirstDbContext.Remove(userInfo);
                var rowAffected = _dbFirstDbContext.SaveChangesAsync();
                return rowAffected.Result;
            }
            return 0;
        }
        #endregion
        #region Interface methods

        User IUser.GetUserInfo(string userid)
        {
            return GetUserInfo(userid);
        }

        public IEnumerable<UserRolesByUser> GetRolesOwneByUser(string userid)
        {
            return GetUserRolesByUserInfos(userid);
        }

        int IUser.RegisterUser(RegisterInfo register)
        {
            return RegisterUser(register);
        }

        UserInfo IUser.GetUserInfoForUpdate(string userId)
        {
            return GetUserInfoForUpdate(userId);
        }

        bool IUser.MatchTheUserInfo(LoginInfo loginInfo)
        {
            return MatchTheUserInfo(loginInfo);
        }

        int IUser.UpdateUser(UserInfo user)
        {
            return UpdateUser(user);
        }

        bool IUser.CompareInfo(UserInfo userInfo)
        {
            return CompareInfo(userInfo);
        }

        int IUser.WithdrawnUser(WithdrawnInfo user)
        {
            return WithdrawnUser(user);
        }
        #endregion
    }
}
