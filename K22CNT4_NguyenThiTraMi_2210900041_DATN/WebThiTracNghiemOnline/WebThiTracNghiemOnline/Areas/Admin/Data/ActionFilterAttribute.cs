using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebThiTracNghiemOnline.Areas.Admin.Data
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session["UserRole"] == null || !_roles.Contains(session["UserRole"].ToString()))
            {
                // Chuyển hướng đến trang đăng nhập nếu người dùng không có quyền
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Login", action = "Login", area = "Admin" }
                    )
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}