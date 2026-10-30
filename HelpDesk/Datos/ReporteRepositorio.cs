using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HelpDesk.Models.ViewModels;

namespace HelpDesk.Datos
{
    
    public class ReporteRepositorio
    {
       
        public List<ConteoEstado> ContarPorEstado()
        {
            const string sql = @"
SELECT e.Id, e.Nombre, COUNT(t.Id) AS Cantidad
FROM dbo.EstadosTicket e
LEFT JOIN dbo.Tickets t ON t.IdEstado = e.Id
GROUP BY e.Id, e.Nombre
ORDER BY e.Id;";

            var lista = new List<ConteoEstado>();
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    lista.Add(new ConteoEstado
                    {
                        IdEstado = dr.Entero("Id"),
                        Estado = dr.Texto("Nombre"),
                        Cantidad = dr.Entero("Cantidad")
                    });
                }
            }
            return lista;
        }

       
        public ResumenPeriodo ObtenerResumen(DateTime desde, DateTime hastaExclusivo)
        {
            const string sql = @"
SELECT
    (SELECT COUNT(*) FROM dbo.Tickets
      WHERE FechaCreacion >= @Desde AND FechaCreacion < @Hasta) AS Creados,
    (SELECT COUNT(*) FROM dbo.Tickets
      WHERE FechaResolucion >= @Desde AND FechaResolucion < @Hasta) AS Resueltos,
    (SELECT AVG(CAST(DATEDIFF(MINUTE, FechaCreacion, FechaResolucion) AS DECIMAL(18, 2))) / 60.0
       FROM dbo.Tickets
      WHERE FechaResolucion >= @Desde AND FechaResolucion < @Hasta) AS PromedioHoras;";

            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = desde;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = hastaExclusivo;
                using (var dr = cmd.ExecuteReader())
                {
                    var resumen = new ResumenPeriodo();
                    if (dr.Read())
                    {
                        resumen.Creados = dr.Entero("Creados");
                        resumen.Resueltos = dr.Entero("Resueltos");
                        int i = dr.GetOrdinal("PromedioHoras");
                        resumen.PromedioHoras = dr.IsDBNull(i) ? (double?)null : (double)dr.GetDecimal(i);
                    }
                    return resumen;
                }
            }
        }

       
        public List<TicketResueltoReporte> ListarResueltos(DateTime desde, DateTime hastaExclusivo)
        {
            const string sql = @"
SELECT t.Id, t.Titulo, a.Nombre AS Asignado, e.Nombre AS Estado, t.IdEstado,
       t.FechaCreacion, t.FechaResolucion,
       CAST(DATEDIFF(MINUTE, t.FechaCreacion, t.FechaResolucion) AS DECIMAL(18, 2)) / 60.0 AS Horas
FROM dbo.Tickets t
LEFT JOIN  dbo.Usuarios a      ON a.Id = t.IdUsuarioAsignado
INNER JOIN dbo.EstadosTicket e ON e.Id = t.IdEstado
WHERE t.FechaResolucion >= @Desde AND t.FechaResolucion < @Hasta
ORDER BY t.FechaResolucion DESC;";

            var lista = new List<TicketResueltoReporte>();
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = desde;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = hastaExclusivo;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TicketResueltoReporte
                        {
                            Id = dr.Entero("Id"),
                            Titulo = dr.Texto("Titulo"),
                            Asignado = dr.Texto("Asignado"),
                            Estado = dr.Texto("Estado"),
                            IdEstado = dr.Entero("IdEstado"),
                            FechaCreacion = dr.Fecha("FechaCreacion"),
                            FechaResolucion = dr.Fecha("FechaResolucion"),
                            HorasResolucion = (double)dr.GetDecimal(dr.GetOrdinal("Horas"))
                        });
                    }
                }
            }
            return lista;
        }
    }
}
