using System.Collections.Generic;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Models.ViewModels
{
 
    public class TablaTicketsViewModel
    {
        public TablaTicketsViewModel()
        {
            Tickets = new List<Ticket>();
        }

        public List<Ticket> Tickets { get; set; }
        public bool MostrarSolicitante { get; set; }
    }

    public class TicketIndexViewModel
    {
        public bool EsAdministrador { get; set; }
        public List<EstadoTicket> Estados { get; set; }
        public TablaTicketsViewModel Tabla { get; set; }
    }

    public class TicketDetalleViewModel
    {
        public Ticket Ticket { get; set; }
        public int IdUsuarioActual { get; set; }
        public bool EsAdministrador { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeComentar { get; set; }

       
        public List<Usuario> PersonalSoporte { get; set; }

    
        public List<EstadoTicket> EstadosSiguientes { get; set; }
    }


    public class MensajeViewModel
    {
        public string Titulo { get; set; }
        public string Texto { get; set; }
    }
}
