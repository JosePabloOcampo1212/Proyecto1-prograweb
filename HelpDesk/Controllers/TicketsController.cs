using System.Web.Mvc;
using HelpDesk.Models.ViewModels;
using HelpDesk.Negocio;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{

    [Autorizar]
    public class TicketsController : BaseController
    {
        private readonly TicketServicio _servicio = new TicketServicio();

        // GET: /Tickets
        public ActionResult Index()
        {
            var usuario = UsuarioActual;
            var modelo = new TicketIndexViewModel
            {
                EsAdministrador = usuario.EsAdministrador,
                Estados = _servicio.ListarEstados(),
                Tabla = new TablaTicketsViewModel
                {
                    Tickets = _servicio.Buscar(usuario, null, null),
                    MostrarSolicitante = usuario.EsAdministrador
                }
            };
            return View(modelo);
        }

        // GET: /Tickets/Buscar?texto=...&idEstado=...

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Buscar(string texto, int? idEstado)
        {
            var usuario = UsuarioActual;
            var tabla = new TablaTicketsViewModel
            {
                Tickets = _servicio.Buscar(usuario, texto, idEstado),
                MostrarSolicitante = usuario.EsAdministrador
            };
            return PartialView("_TablaTickets", tabla);
        }
    }
}
