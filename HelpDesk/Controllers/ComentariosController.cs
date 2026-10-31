using System.Linq;
using System.Web.Mvc;
using HelpDesk.Helpers;
using HelpDesk.Negocio;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{
   
    [Autorizar]
    public class ComentariosController : BaseController
    {
        private readonly ComentarioServicio _servicio = new ComentarioServicio();

        // GET: /Comentarios/Listar?idTicket=5
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Listar(int idTicket)
        {
            var comentarios = _servicio.Listar(idTicket, UsuarioActual);
            return Json(new
            {
                exito = true,
                comentarios = comentarios.Select(DatosJson.Comentario).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: /Comentarios/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Agregar(int idTicket, string texto)
        {
            var comentario = _servicio.Agregar(idTicket, texto, UsuarioActual);
            return Json(new
            {
                exito = true,
                mensaje = "Comentario agregado.",
                comentario = DatosJson.Comentario(comentario),
                fechaActualizacion = Formato.Fecha(comentario.Fecha)
            });
        }
    }
}
