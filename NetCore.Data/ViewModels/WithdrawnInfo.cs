using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class WithdrawnInfo
    {
        /// <summary>
        /// 사용자 아이디
        /// </summary>
        public string? UserId { get; set; }
        [Required(ErrorMessage = "비밀번호를 입력하세요")]
        [Display(Name = "비밀번호")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상 입력하세요")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
