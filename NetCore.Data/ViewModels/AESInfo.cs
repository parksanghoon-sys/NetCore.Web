﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.ViewModels
{
    public class AESInfo
    {
        [Required(ErrorMessage = "아이디를 입력하세요.")]
        [MinLength(4, ErrorMessage = "아이디는 4자 이상 입력하세요.")]
        [Display(Name = "사용자 아이디")]
        public string? UserId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "비밀번호는 6자 이상 입력하세요.")]
        [Display(Name = "비밀번호")]
        public string? Password { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "암호화 정보")]
        public string? EncUserInfo { get; set; }

        [Display(Name = "복호화 정보")]
        public string? DecUserInfo { get; set; }
    }
}
