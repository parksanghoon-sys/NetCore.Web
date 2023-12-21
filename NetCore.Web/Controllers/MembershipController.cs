using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace NetCore.Web.Controllers
{
    /// <summary>
    /// 위해당하는 인증레벨이 아니면 접속이 불가능함
    /// </summary>
    [Authorize(Roles = "AssociateUser,GeneralUser,SuperUser,SystemUser")]
    public class MembershipController : Controller
    {
        private readonly IUser? _user;
        private readonly HttpContext? _context;
        private readonly IPasswordHasher _passwordHasher;
        public MembershipController(IHttpContextAccessor accessor,IUser user, IPasswordHasher passwordHasher)
        {
            _context = accessor.HttpContext;
            _passwordHasher = passwordHasher;
            _user = user;

        }
        #region private methods
        /// <summary>
        /// 로컬 URL 인지 외부 URL인지 체크
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if(Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(MembershipController.Index), "Membership");
            }
        }
        #endregion
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Login Page Http Get을 받음
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Login")]
        [AllowAnonymous] // 모두 접근 가능
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// ValidateAntiForgeryToken 는 위조방지 토큰을 통해 View로부터 받은 Post data가 유효한지 검증
        /// </summary>
        /// <returns></returns>
        [HttpPost("/Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // 모두 접근 가능
        public async Task<IActionResult> LoginAsync(LoginInfo loginInfo, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;
            /// ModelState 란 Http 요청에서 엑션 메서드 값으로 값 바인딩을 시도할떄의 상태 
            /// ModelState는 모델 바인딩과 모델 유효성 검사에서 발생하는 오류를 나타낸다 
            /// POST 된 이름 값 쌍을 저장하고 제출하며 각 값과 관련되 유효성 검사오류를 저장한다
            if(ModelState.IsValid)
            {           
                //if (_user.MatchTheUserInfo(loginInfo))
                if(_user.MatchTheUserInfo(loginInfo))
                {
                    // 신원 보증과 승인 권환
                    var userInfo = _user.GetUserInfo(loginInfo.UserId);
                    var rolse = _user.GetRolesOwneByUser(loginInfo.UserId);
                    var userTopRole = rolse!.FirstOrDefault();
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" +
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    var identity = new ClaimsIdentity(claims: new[]
                     {
                    new Claim(type:ClaimTypes.Name,
                              value:userInfo.UserId),
                    new Claim(type:ClaimTypes.Role,
                              value:userTopRole.RoleId),
                    new Claim(type:ClaimTypes.UserData,
                              value:userDataInfo)
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: new ClaimsPrincipal(identity: identity),
                        properties: new AuthenticationProperties()
                    {
                        IsPersistent = loginInfo.RememberMe, // 지속 여부에 관한 옵션
                        ExpiresUtc = loginInfo.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30) // 체크시 30일 아닐시 7일
                    });
                    /// TempData 란 현재 요청 이후에만 사용할수 있는 일시적인 데이터를 저장, 
                    /// Controler에서 사용가능하며 컨트롤러에서 뷰로 데이터를 전달하거나 뷰에서 컨트롤로 전달할때 사용
                    /// 일시적인 데이터를 저장하며 값을 검색후 자동 제거 => 저장 객체가 읽으면 해당 요청의 끝에서 삭제
                    TempData["Message"] = "로그인이 성공적으로 이루어 졌습니다.";

                    //return RedirectToAction("Index", "Membership");
                    // 모델 데이터가 정상적일시 index의 membership 페이지로 이동한다.

                    return RedirectToLocal(returnUrl);
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
        [HttpGet]
        public IActionResult Register(string returnRul)
        {
            ViewData["ReturnUrl"] = returnRul;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterInfo registerInfo, string returnRul)
        {
            ViewData["RetrunUrl"] = registerInfo;
            string message = string.Empty;
            if(ModelState.IsValid)
            {
                // 사용자 가입 서비스
                TempData["Message"] = "사용자 가입이 성공적으로 이루어 졌습니다";
                return RedirectToAction("Login", "Membership");
            }
            else
            {
                message = "사용자 가입을 위한 정보를 올바르게 입력하세요.";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(registerInfo);
        }
        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Logout Sucess <br/> 웹사이트를 원활히 이용하시려면 로그인 하세요.";
            return RedirectToAction("Index", "Membership");
        }

        [HttpGet("/Forbidden")]             
        public IActionResult Forbidden()
        {
            StringValues paramReturnUrl;
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            ViewData["Message"] = $"귀하는 {paramReturnUrl} 경로로 접근하려고 했습니다 만 <br/>" +
                                   "인증된 사용자도 접근하지 못하는 페이지가 있습니다 <br/>" +
                                   "담당자에게 해당 페이지의 접근권한에 대해 문의하세요";
            
            return View();
        }
    }
}
