using System.Collections.Generic;
using System.Data.SqlClient;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Datos
{
  
    public class EstadoTicketRepositorio
    {
        public List<EstadoTicket> Listar()
        {
            var lista = new List<EstadoTicket>();
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand("SELECT Id, Nombre FROM dbo.EstadosTicket ORDER BY Id;", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    lista.Add(new EstadoTicket { Id = dr.Entero("Id"), Nombre = dr.Texto("Nombre") });
                }
            }
            return lista;
        }
    }
}
