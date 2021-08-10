using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestCore.Helper
{
    public class AuthRequire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            byte[] value;
            object data = context.HttpContext.Session.TryGetValue("login", out value);

            if(value == null || value.Length <1 )
            {
                context.Result = new RedirectResult("/Home/Login");
            }

            base.OnActionExecuting(context);
        }
    }
}
