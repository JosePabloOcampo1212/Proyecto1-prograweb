using System;
using System.Collections.Generic;
using System.Linq;

namespace HelpDesk.Models.ViewModels
{
    public class ConteoEstado
    {
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public int Cantidad { get; set; }
    }

    public class TicketResueltoReporte
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Asignado { get; set; }
        public string Estado { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaResolucion { get; set; }
        public double HorasResolucion { get; set; }
    }

    public class ResumenPeriodo
    {
        public int Creados { get; set; }
        public int Resueltos { get; set; }
        public double? PromedioHoras { get; set; }
    }

    public class ReporteViewModel
    {
        public ReporteViewModel()
        {
            PorEstado = new List<ConteoEstado>();
            TicketsResueltos = new List<TicketResueltoReporte>();
            Resumen = new ResumenPeriodo();
        }

        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string Error { get; set; }

        /// <summary>Situación actual: cantidad de tickets en cada estado.</summary>
        public List<ConteoEstado> PorEstado { get; set; }

        /// <summary>Totales del periodo consultado.</summary>
        public ResumenPeriodo Resumen { get; set; }

        /// <summary>Tickets resueltos dentro del periodo con su tiempo de resolución.</summary>
        public List<TicketResueltoReporte> TicketsResueltos { get; set; }

        public int TotalTickets
        {
            get { return PorEstado.Sum(e => e.Cantidad); }
        }
    }
}
