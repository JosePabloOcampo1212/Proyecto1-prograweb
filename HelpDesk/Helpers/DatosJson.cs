using System.Collections.Generic;
using System.Linq;
using HelpDesk.Models;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Helpers
{

    public static class DatosJson
    {
        public static object Comentario(Comentario c)
        {
            return new
            {
                id = c.Id,
                autor = c.NombreUsuario,
                esSoporte = c.RolUsuario == RolesSistema.Administrador,
                texto = c.Texto,
                fecha = Formato.Fecha(c.Fecha)
            };
        }

        public static object Ticket(Ticket t, List<EstadoTicket> estadosSiguientes)
        {
            return new
            {
                id = t.Id,
                idEstado = t.IdEstado,
                estado = t.NombreEstado,
                claseEstado = EstiloEstado.Clase(t.IdEstado),
                idAsignado = t.IdUsuarioAsignado,
                asignado = t.NombreAsignado,
                fechaActualizacion = Formato.Fecha(t.FechaActualizacion),
                fechaResolucion = Formato.Fecha(t.FechaResolucion),
                puedeComentar = t.IdEstado != Estados.Cerrado,
                estadosSiguientes = estadosSiguientes.Select(e => new { id = e.Id, nombre = e.Nombre }).ToList()
            };
        }
    }
}
