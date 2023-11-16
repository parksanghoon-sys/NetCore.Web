using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace NetCore.Web.Controllers
{
    public class MembershipController : Controller
    {
        private readonly IUser? _user;
        private readonly HttpContext? _context;
        public MembershipController(IHttpContextAccessor accessor,IUser user)
        {
            _context = accessor.HttpContext;
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
        [HttpGet("/Login")]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// ValidateAntiForgeryToken 는 위조방지 토큰을 통해 View로부터 받은 Post data가 유효한지 검증
        /// </summary>
        /// <returns></returns>
        [HttpPost("/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginInfo loginInfo)
        {
            string message = string.Empty;
            /// ModelState 란 Http 요청에서 엑션 메서드 값으로 값 바인딩을 시도할떄의 상태 
            /// ModelState는 모델 바인딩과 모델 유효성 검사에서 발생하는 오류를 나타낸다 
            /// POST 된 이름 값 쌍을 저장하고 제출하며 각 값과 관련되 유효성 검사오류를 저장한다
            if(ModelState.IsValid)
            {
                if (_user.MatchTheUserInfo(loginInfo))
                {
                    // 신원 보증과 승인 권환
                    var userInfo = _user.GetUserInfo(loginInfo.UserId);
                    var rolse = _user.GetRolesOwneByUser(loginInfo.UserId);
                    var userTopRole = rolse!.FirstOrDefault();

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type:ClaimTypes.Name, value:userInfo.UserName),
                        new Claim(type:ClaimTypes.Role, value:userTopRole.RoleId + "|" + userTopRole.UserRole.RoleName + "|" + userTopRole.UserRole.RolePriority.ToString())
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme 
                        ,principal:new ClaimsPrincipal(identities: (IEnumerable<ClaimsIdentity>)identity)
                        ,properties: new AuthenticationProperties()
                    {
                        IsPersistent = loginInfo.RememberMe, // 지속 여부에 관한 옵션
                        ExpiresUtc = loginInfo.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30) // 체크시 30일 아닐시 7일
                    });
                    /// TempData 란 현재 요청 이후에만 사용할수 있는 일시적인 데이터를 저장, 
                    /// Controler에서 사용가능하며 컨트롤러에서 뷰로 데이터를 전달하거나 뷰에서 컨트롤로 전달할때 사용
                    /// 일시적인 데이터를 저장하며 값을 검색후 자동 제거 => 저장 객체가 읽으면 해당 요청의 끝에서 삭제
                    TempData["Message"] = "로그인이 성공적으로 이루어 졌습니다.";
                    return RedirectToAction("Index", "Membership");
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
            return View("Login",loginInfo);
        }
        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Logout Sucess <br/> 웹사이트를 원활히 이용하시려면 로그인 하세요.";
            return RedirectToAction("Index", "Membership");
        }
    }
}
