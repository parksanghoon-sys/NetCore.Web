using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.ViewModels
{
    public class LoginInfo
    {
        /// <summary>
        /// UserId 항목이 Display시 사용할문구
        /// 반드시 필요하고 에러시 메세지 문구
        /// 최수 글자수 입력
        /// </summary>
        [Display(Name = "사용자 아이디")]
        [Required(ErrorMessage = "사용자 아이디를 입력하세요")]
        [MinLength(6 , ErrorMessage = "사용자 아이디는 최소 6자 이상 입력하세요")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "비밀번호를 입력하세요")]
        [Display(Name = "비밀번호")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상 입력하세요")]
        public string Password { get; set; }
    }
}
