using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Web.Extensions;

namespace NetCore.Web.Controllers
{
    public class DataController : Controller
    {
        private IDataProtector _protector;
        private readonly HttpContext _context;
        private string _sessionKeyCarName = "_sessionCartKey";
        public DataController(IDataProtectionProvider provider, IHttpContextAccessor httpContext)
        {
            _protector = provider.CreateProtector("NetCore.Data.v1");
            _context = httpContext.HttpContext;
        }
        #region private Methods
        private List<ItemInfo> GetCartInfos(ref string message)
        {
            var cartInfos = _context.Session.Get<List<ItemInfo>>(key: _sessionKeyCarName);
            if(cartInfos == null || cartInfos.Count() < 1)
            {
                message = "장바구니에 담긴 상품이 없습니다";
            }
            return cartInfos;
        }
        private void SetCartInfos(ItemInfo item, List<ItemInfo> cartInfos = null)
        {
            if(cartInfos == null)
            {
                cartInfos = _context.Session.Get<List<ItemInfo>>(_sessionKeyCarName);
                if(cartInfos == null)
                {
                    cartInfos = new List<ItemInfo>();
                }    
            }
            cartInfos.Add(item);
            _context.Session.Set<List<ItemInfo>>(_sessionKeyCarName, cartInfos);
        }
        #endregion
        #region AES
        [HttpGet]
        [Authorize(Roles = "SuperUser, SystemUser")]
        public IActionResult AES()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperUser, SystemUser")]
        public IActionResult AES(AESInfo aes)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                string userInfo = aes.UserId + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo);//암호화 정보
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo);//복호화 정보

                ViewData["Message"] = "암복호화가 성공적으로 이루어졌습니다.";

                return View(aes);
            }
            else
            {
                var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();
                message = "암복호화를 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(aes);
        }
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCart()
        {
            SetCartInfos(new ItemInfo()
            {
                ItemNo = Guid.NewGuid(),
                ItemName = DateTime.UtcNow.Ticks.ToString()
            });
            return (RedirectToAction("Cart", "Data"));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCart()
        {
            string message = string.Empty;
            var carInfos = GetCartInfos(ref message);
            if(carInfos != null && carInfos.Count > 0)
            {
                _context.Session.Remove(key: _sessionKeyCarName);
            }
            return (RedirectToAction("Cart", "Data"));
        }
        public IActionResult Cart()
        {
            string message = string.Empty;
            var carInfos = GetCartInfos(ref message);

            ViewData["Message"] = message;
            return View(carInfos);
        }
    }
}
