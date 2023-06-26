using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;

namespace NetCore.Web.Controllers
{
    public class MembershipController : Controller
    {
        private IUser _user;
        public MembershipController(IUser user)
        {
            _user = user;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Login Page Http Get을 받음
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// ValidateAntiForgeryToken 는 위조방지 토큰을 통해 View로부터 받은 Post data가 유효한지 검증
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginInfo loginInfo)
        {
            string message = string.Empty;
            /// ModelState 란 Http 요청에서 엑션 메서드 값으로 값 바인딩을 시도할떄의 상태 
            /// ModelState는 모델 바인딩과 모델 유효성 검사에서 발생하는 오류를 나타낸다 
            /// POST 된 이름 값 쌍을 저장하고 제출하며 각 값과 관련되 유효성 검사오류를 저장한다
            if(ModelState.IsValid)
            {
                if (_user.MatchTheUserInfo(loginInfo))
                {
                    TempData["Message"] = "로그인이 성공적으로 이루어 졌습니다.";
                    return RedirectToAction("Index", "Membership");
                    /// TempData 란 현재 요청 이후에만 사용할수 있는 일시적인 데이터를 저장, 
                    /// Controler에서 사용가능하며 컨트롤러에서 뷰로 데이터를 전달하거나 뷰에서 컨트롤로 전달할때 사용
                    /// 일시적인 데이터를 저장하며 값을 검색후 자동 제거 => 저장 객체가 읽으면 해당 요청의 끝에서 삭제
                    // 모델 데이터가 정상적일시 index의 membership 페이지로 이동한다.
                }
                else
                    message = "로그인 되지 않았습니다";
            }
            else
            {
                message = "로그인 정보를 올바르게 입력하세요";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(loginInfo);
        }
    }
}
