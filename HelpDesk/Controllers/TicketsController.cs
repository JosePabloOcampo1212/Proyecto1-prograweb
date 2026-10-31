using System.Collections.Generic;
using System.Web.Mvc;
using HelpDesk.Helpers;
using HelpDesk.Models;
using HelpDesk.Models.Entidades;
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

        // GET: /Tickets/Detalle/5
        [HttpGet]
        public ActionResult Detalle(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var usuario = UsuarioActual;
            var ticket = _servicio.ObtenerConAcceso(id.Value, usuario);

            var modelo = new TicketDetalleViewModel
            {
                Ticket = ticket,
                IdUsuarioActual = usuario.Id,
                EsAdministrador = usuario.EsAdministrador,
                PuedeEditar = _servicio.PuedeEditar(ticket, usuario),
                PuedeComentar = ComentarioServicio.PuedeComentar(ticket),
                PersonalSoporte = usuario.EsAdministrador ? _servicio.ListarPersonalSoporte() : new List<Usuario>(),
                EstadosSiguientes = usuario.EsAdministrador ? _servicio.EstadosSiguientes(ticket.IdEstado) : new List<EstadoTicket>()
            };
            return View(modelo);
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

        // POST: /Tickets/Asignar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Autorizar(RolesSistema.Administrador)]
        public JsonResult Asignar(int id, int? idUsuarioAsignado)
        {
            if (!idUsuarioAsignado.HasValue)
            {
                throw new ReglaNegocioException("Seleccione la persona de soporte a la que se asignará el ticket.");
            }

            var ticket = _servicio.Asignar(id, idUsuarioAsignado.Value, UsuarioActual);
            return Json(new
            {
                exito = true,
                mensaje = "Ticket asignado a " + ticket.NombreAsignado + ".",
                ticket = DatosJson.Ticket(ticket, _servicio.EstadosSiguientes(ticket.IdEstado))
            });
        }

        // POST: /Tickets/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Autorizar(RolesSistema.Administrador)]
        public JsonResult CambiarEstado(int id, int? idEstado)
        {
            if (!idEstado.HasValue)
            {
                throw new ReglaNegocioException("Seleccione el nuevo estado.");
            }

            var ticket = _servicio.CambiarEstado(id, idEstado.Value, UsuarioActual);
            return Json(new
            {
                exito = true,
                mensaje = "El ticket pasó a " + ticket.NombreEstado + ".",
                ticket = DatosJson.Ticket(ticket, _servicio.EstadosSiguientes(ticket.IdEstado))
            });
        }

        // POST: /Tickets/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Autorizar(RolesSistema.Administrador)]
        public ActionResult Eliminar(int id)
        {
            _servicio.Eliminar(id, UsuarioActual);
            TempData["Mensaje"] = "El ticket #" + id + " y sus comentarios se eliminaron.";
            return RedirectToAction("Index");
        }
    }
}
