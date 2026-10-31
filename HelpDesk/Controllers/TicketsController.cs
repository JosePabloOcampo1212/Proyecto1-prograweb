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

        // GET: /Tickets/Crear
        [HttpGet]
        public ActionResult Crear()
        {
            return View(new TicketFormViewModel());
        }

        // POST: /Tickets/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(TicketFormViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                int id = _servicio.Crear(modelo, UsuarioActual);
                TempData["Mensaje"] = "El ticket #" + id + " se registró correctamente.";
                return RedirectToAction("Detalle", new { id = id });
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(modelo);
            }
        }

        // GET: /Tickets/Editar/5
        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var ticket = _servicio.ObtenerConAcceso(id.Value, UsuarioActual);
            if (!_servicio.PuedeEditar(ticket, UsuarioActual))
            {
                TempData["Error"] = "Solo se pueden editar los tickets propios que estén en estado Abierto.";
                return RedirectToAction("Detalle", new { id = ticket.Id });
            }

            return View(new TicketFormViewModel
            {
                Id = ticket.Id,
                Titulo = ticket.Titulo,
                Descripcion = ticket.Descripcion,
                Categoria = ticket.Categoria
            });
        }

        // POST: /Tickets/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(TicketFormViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                _servicio.Editar(modelo.Id, modelo, UsuarioActual);
                TempData["Mensaje"] = "Los cambios del ticket #" + modelo.Id + " se guardaron.";
                return RedirectToAction("Detalle", new { id = modelo.Id });
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(modelo);
            }
        }
    }
}
