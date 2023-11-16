using NetCore.Data.Classes;
using NetCore.Data.ViewModels;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfo loginInfo);
        User GetUserInfo(string userid);
        IEnumerable<UserRolesByUser> GetRolesOwneByUser(string userid);
    }
}
