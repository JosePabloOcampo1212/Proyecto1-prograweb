using System.Web.Mvc;
using HelpDesk.Helpers;

namespace HelpDesk.Seguridad
{
    
    public class AutorizarAttribute : AuthorizeAttribute
    {
        public AutorizarAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var http = filterContext.HttpContext;

            if (http.Request.IsAuthenticated)
            {
                filterContext.Result = RespuestaError.Crear(filterContext, 403,
                    "No tiene permisos para realizar esta acción. Es exclusiva del rol Administrador.");
                return;
            }

            if (http.Request.IsAjaxRequest())
            {
                
                http.Response.SuppressFormsAuthenticationRedirect = true;
                filterContext.Result = RespuestaError.Crear(filterContext, 401,
                    "La sesión expiró. Vuelva a iniciar sesión.");
                return;
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
