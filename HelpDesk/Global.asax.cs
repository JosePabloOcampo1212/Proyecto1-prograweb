using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using HelpDesk.Seguridad;

namespace HelpDesk
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // No anunciar la versión de MVC en los encabezados de respuesta.
            MvcHandler.DisableMvcResponseHeader = true;

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Después de que Forms Authentication valida la cookie, se reemplaza el usuario
        /// por un UsuarioPrincipal con el Id, el nombre y el rol guardados en la cookie.
        /// Así User.IsInRole y los filtros [Autorizar(...)] funcionan con los roles del sistema.
        /// </summary>
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var contexto = Context;
            if (contexto.User == null || !contexto.User.Identity.IsAuthenticated)
            {
                return;
            }

            var identidad = contexto.User.Identity as FormsIdentity;
            var principal = UsuarioPrincipal.DesdeIdentidad(identidad);

            if (principal == null)
            {
                // Cookie con datos incompletos: se cierra la sesión y se trata como anónimo.
                FormsAuthentication.SignOut();
                var anonimo = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
                contexto.User = anonimo;
                Thread.CurrentPrincipal = anonimo;
                return;
            }

            contexto.User = principal;
            Thread.CurrentPrincipal = principal;
        }
    }
}
