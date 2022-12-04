using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace Cj.web.protal.App_Start
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["LoggedUserName"] == null || HttpContext.Current.Session["LogedUserPassword"] == null)
            {
                //filterContext.Result = new ViewResult
                //{
                //    ViewName = "Error"
                //};
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "action", "Login" },
                    { "controller", "User" },
                    { "returnUrl", filterContext.HttpContext.Request.RawUrl}
                });
            }
            return;
        }
    }
    public class PermishonFilter : ActionFilterAttribute
    {
        public string Code { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!((IList<string>)HttpContext.Current.Session["Permissions"]).Contains(Code))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "action", "PageNotFound" },
                    { "controller", "Common" }
                    //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}
                });
            }
            return;
        }
    }
}