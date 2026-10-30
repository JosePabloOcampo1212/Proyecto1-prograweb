using System.Web.Mvc;
using HelpDesk.Models.ViewModels;

namespace HelpDesk.Helpers
{

    public static class RespuestaError
    {
        public static ActionResult Crear(ControllerContext contexto, int codigoHttp, string mensaje)
        {
            var http = contexto.HttpContext;
            http.Response.StatusCode = codigoHttp;
            http.Response.TrySkipIisCustomErrors = true;

            if (http.Request.IsAjaxRequest())
            {
                return new JsonResult
                {
                    Data = new { exito = false, mensaje = mensaje },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var modelo = new MensajeViewModel { Titulo = TituloPara(codigoHttp), Texto = mensaje };
            return new ViewResult
            {
                ViewName = "~/Views/Shared/Mensaje.cshtml",
                ViewData = new ViewDataDictionary<MensajeViewModel>(modelo),
                TempData = contexto.Controller.TempData
            };
        }

        private static string TituloPara(int codigoHttp)
        {
            switch (codigoHttp)
            {
                case 400: return "No se pudo completar la operación";
                case 401: return "Sesión expirada";
                case 403: return "Acceso denegado";
                case 404: return "No encontrado";
                default: return "Aviso";
            }
        }
    }
}
