using NetCore.Data.Classes;
using NetCore.Data.ViewModels;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfo loginInfo);
        User GetUserInfo(string userid);
        IEnumerable<UserRolesByUser> GetRolesOwneByUser(string userid);
        /// <summary>
        /// [사용자 가입]
        /// </summary>
        /// <param name="register">사용자 가입용 뷰모델</param>
        /// <returns></returns>
        int RegisterUser(RegisterInfo register);
        /// <summary>
        /// [사용자 정보 수정을 위한 검색]
        /// </summary>
        /// <param name="userId">사용자 아이디</param>
        /// <returns></returns>
        UserInfo GetUserInfoForUpdate(string userId);
        /// <summary>
        /// [사용자 정보 수정]
        /// </summary>
        int UpdateUser(UserInfo user);
        /// <summary>
        /// [사용자 정보 수정에서 변경대상 비교] 같으면 true 다르면 false
        /// </summary>
        /// <param name="userInfo">사용자 정보 뷰모델</param>
        /// <returns></returns>
        bool CompareInfo(UserInfo userInfo);
        /// <summary>
        /// 사용자 탈퇴
        /// </summary>
        /// <param name="user">사용자 탈퇴정보 viewmodel</param>
        /// <returns></returns>
        int WithdrawnUser(WithdrawnInfo user);
    }
}
