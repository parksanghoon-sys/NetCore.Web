using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.ViewModels
{
    public class UserInfo
    {
        [Display(Name = "사용자 아이디")]
        [Required(ErrorMessage = "사용자 아이디를 입력하세요")]
        [MinLength(6, ErrorMessage = "사용자 아이디는 최소 6자 이상 입력하세요")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력하세요")]
        [Display(Name = "비밀번호")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상 입력하세요")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "내정보 기억")]
        public bool RememberMe { get; set; }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        
        [Required(ErrorMessage = "사용자 이름을 입력하세요")]
        [Display(Name = "사용자 이름")]
        public string? UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "사용자 이메일을 입력하세요")]
        [Display(Name = "사용자 Email")]
        public string? UserEmail { get; set; }
        public virtual ChangeInfo? ChangeInfo { get; set; }
    }
}
