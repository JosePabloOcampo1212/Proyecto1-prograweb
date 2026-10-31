using System;
using HelpDesk.Datos;
using HelpDesk.Models.ViewModels;
using HelpDesk.Seguridad;

namespace HelpDesk.Negocio
{
    
    public class ReporteServicio
    {
        private readonly ReporteRepositorio _reportes = new ReporteRepositorio();

        public ReporteViewModel Generar(DateTime? desde, DateTime? hasta, UsuarioPrincipal usuario)
        {
            TicketServicio.ExigirAdministrador(usuario);

            var hoy = DateTime.Today;
            var modelo = new ReporteViewModel
            {
                Desde = (desde ?? hoy.AddDays(-30)).Date,
                Hasta = (hasta ?? hoy).Date
            };

            modelo.PorEstado = _reportes.ContarPorEstado();

            if (modelo.Desde > modelo.Hasta)
            {
                modelo.Error = "La fecha inicial no puede ser posterior a la fecha final.";
                return modelo;
            }

       
            var hastaExclusivo = modelo.Hasta.AddDays(1);
            modelo.Resumen = _reportes.ObtenerResumen(modelo.Desde, hastaExclusivo);
            modelo.TicketsResueltos = _reportes.ListarResueltos(modelo.Desde, hastaExclusivo);
            return modelo;
        }
    }
}
