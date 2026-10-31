using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using HelpDesk.Helpers;
using HelpDesk.Negocio;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{

    public abstract class BaseController : Controller
    {
        protected UsuarioPrincipal UsuarioActual
        {
            get { return User as UsuarioPrincipal; }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var ex = filterContext.Exception;
            int codigo;
            string mensaje;

            if (ex is AccesoDenegadoException)
            {
                codigo = 403;
                mensaje = ex.Message;
            }
            else if (ex is RecursoNoEncontradoException)
            {
                codigo = 404;
                mensaje = ex.Message;
            }
            else if (ex is ReglaNegocioException)
            {
                codigo = 400;
                mensaje = ex.Message;
            }
            else if (ex is HttpRequestValidationException)
            {
                codigo = 400;
                mensaje = "El texto contiene caracteres que no se permiten, como < o >. Elimínelos e intente de nuevo.";
            }
            else if (ex is HttpAntiForgeryException)
            {
                codigo = 400;
                mensaje = "La página expiró o la sesión cambió. Recargue la página e intente de nuevo.";
            }
            else if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                Trace.TraceError(ex.ToString());
                codigo = 500;
                mensaje = "Ocurrió un error inesperado. Intente de nuevo en unos segundos.";
            }
            else
            {
                
                return;
            }

            filterContext.ExceptionHandled = true;
            filterContext.Result = RespuestaError.Crear(filterContext, codigo, mensaje);
        }
    }
}
