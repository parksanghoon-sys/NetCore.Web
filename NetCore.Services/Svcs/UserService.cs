using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        private  DBFirstDbContext _dbFirstDbContext;
        private  CodeFirstDbContext _codeFirstDbContext;
        public UserService(CodeFirstDbContext codeFirstDbContext)
        {
            _codeFirstDbContext = codeFirstDbContext;
        }
        public bool MatchTheUserInfo(LoginInfo loginInfo)
        {
            return checkTheUserInfo(loginInfo.UserId, loginInfo.Password);
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
            return _codeFirstDbContext.Users.ToList();
            //return _dbFirstDbContext.Users.ToList();
        }
        private bool checkTheUserInfo(string userid, string password)
        {
            return GetUserInfos().Where(u => u.UserId.Equals(userid) && u.Password.Equals(password)).Any();
        }
        #endregion
    }
}
