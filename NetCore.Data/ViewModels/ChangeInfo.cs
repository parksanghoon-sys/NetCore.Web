using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class ChangeInfo
    {        
        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string? UserName { get; set; }     
        /// <summary>
        /// 사용자 이메일
        /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// true : 전부 일치 시 
        /// false : 불 일치 시
        /// </summary>
        /// <param name="userInfo">비교 클래스</param>
        /// <returns></returns>
        public bool Equals(UserInfo userInfo)
        {
            if(!string.Equals(UserName,userInfo.UserName, StringComparison.OrdinalIgnoreCase))
                return false;
            if (!string.Equals(UserEmail, userInfo.UserEmail, StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }
    }
}
