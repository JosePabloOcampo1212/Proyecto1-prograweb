using System;
using System.Web.Mvc;
using HelpDesk.Models;
using HelpDesk.Negocio;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{
   
    [Autorizar(RolesSistema.Administrador)]
    public class ReportesController : BaseController
    {
        private readonly ReporteServicio _servicio = new ReporteServicio();

        // GET: /Reportes?desde=2026-10-01&hasta=2026-10-31
        [HttpGet]
        public ActionResult Index(DateTime? desde, DateTime? hasta)
        {
            var modelo = _servicio.Generar(desde, hasta, UsuarioActual);
            return View(modelo);
        }
    }
}
