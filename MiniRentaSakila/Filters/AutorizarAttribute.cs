namespace MiniRentaSakila.Filters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using MiniRentaSakila.Helpers;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutorizarAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AutorizarAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string rol = context.HttpContext.Session.GetString(SessionKeys.Rol);

            if (rol == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(rol))
            {
                context.Result = new RedirectToActionResult("AccesoDenegado", "Account", null);
            }
        }
    }
}
