using System;

namespace HelpDesk.Models.Entidades
{
   
    public class Ticket
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int IdUsuarioSolicitante { get; set; }
        public int? IdUsuarioAsignado { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime? FechaResolucion { get; set; }

        
        public string NombreSolicitante { get; set; }
        public string NombreAsignado { get; set; }
        public string NombreEstado { get; set; }
    }
}
