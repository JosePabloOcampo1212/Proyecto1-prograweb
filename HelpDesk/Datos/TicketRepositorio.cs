using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Datos
{
    
    public class TicketRepositorio
    {
        private const string SelectBase = @"
SELECT t.Id, t.Titulo, t.Descripcion, t.Categoria,
       t.IdUsuarioSolicitante, t.IdUsuarioAsignado, t.IdEstado,
       t.FechaCreacion, t.FechaActualizacion, t.FechaResolucion,
       s.Nombre AS NombreSolicitante,
       a.Nombre AS NombreAsignado,
       e.Nombre AS NombreEstado
FROM dbo.Tickets t
INNER JOIN dbo.Usuarios s      ON s.Id = t.IdUsuarioSolicitante
LEFT JOIN  dbo.Usuarios a      ON a.Id = t.IdUsuarioAsignado
INNER JOIN dbo.EstadosTicket e ON e.Id = t.IdEstado";

        public Ticket ObtenerPorId(int id)
        {
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + " WHERE t.Id = @Id;", cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                using (var dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }


        public List<Ticket> Buscar(int? idSolicitante, int? idEstado, string patronTitulo, int? numero)
        {
            const string filtros = @"
WHERE (@IdSolicitante IS NULL OR t.IdUsuarioSolicitante = @IdSolicitante)
  AND (@IdEstado IS NULL OR t.IdEstado = @IdEstado)
  AND (@Patron IS NULL OR t.Titulo LIKE @Patron ESCAPE '\' OR t.Id = @Numero)
ORDER BY t.Id DESC;";

            var lista = new List<Ticket>();
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + filtros, cn))
            {
                cmd.Parameters.Add("@IdSolicitante", SqlDbType.Int).Value = LectorExtensiones.ValorONulo(idSolicitante);
                cmd.Parameters.Add("@IdEstado", SqlDbType.Int).Value = LectorExtensiones.ValorONulo(idEstado);
                cmd.Parameters.Add("@Patron", SqlDbType.NVarChar, 250).Value = LectorExtensiones.ValorONulo(patronTitulo);
                cmd.Parameters.Add("@Numero", SqlDbType.Int).Value = LectorExtensiones.ValorONulo(numero);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Mapear(dr));
                    }
                }
            }
            return lista;
        }

        public int Insertar(Ticket ticket)
        {
            const string sql = @"
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado,
                         FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (@Titulo, @Descripcion, @IdUsuarioSolicitante, NULL, @IdEstado,
        @Fecha, @Fecha, @Categoria, NULL);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = ticket.Titulo;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = ticket.Descripcion;
                cmd.Parameters.Add("@IdUsuarioSolicitante", SqlDbType.Int).Value = ticket.IdUsuarioSolicitante;
                cmd.Parameters.Add("@IdEstado", SqlDbType.Int).Value = ticket.IdEstado;
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = ticket.FechaCreacion;
                cmd.Parameters.Add("@Categoria", SqlDbType.NVarChar, 50).Value = ticket.Categoria;
                return (int)cmd.ExecuteScalar();
            }
        }


        public int ActualizarDatos(Ticket ticket)
        {
            const string sql = @"
UPDATE dbo.Tickets
SET Titulo = @Titulo, Descripcion = @Descripcion, Categoria = @Categoria, FechaActualizacion = @Fecha
WHERE Id = @Id AND IdEstado = @IdEstadoAbierto;";

            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = ticket.Id;
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = ticket.Titulo;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = ticket.Descripcion;
                cmd.Parameters.Add("@Categoria", SqlDbType.NVarChar, 50).Value = ticket.Categoria;
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = ticket.FechaActualizacion;
                cmd.Parameters.Add("@IdEstadoAbierto", SqlDbType.Int).Value = Models.Estados.Abierto;
                return cmd.ExecuteNonQuery();
            }
        }

 
        public int ActualizarAsignacion(int idTicket, int idUsuarioAsignado, DateTime fecha)
        {
            const string sql = @"
UPDATE dbo.Tickets
SET IdUsuarioAsignado = @IdUsuarioAsignado, FechaActualizacion = @Fecha
WHERE Id = @Id AND IdEstado <> @IdEstadoCerrado;";

            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idTicket;
                cmd.Parameters.Add("@IdUsuarioAsignado", SqlDbType.Int).Value = idUsuarioAsignado;
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
                cmd.Parameters.Add("@IdEstadoCerrado", SqlDbType.Int).Value = Models.Estados.Cerrado;
                return cmd.ExecuteNonQuery();
            }
        }

    
        public int ActualizarEstado(int idTicket, int idEstadoActual, int idEstadoNuevo, DateTime? fechaResolucion, DateTime fecha)
        {
            const string sql = @"
UPDATE dbo.Tickets
SET IdEstado = @IdEstadoNuevo, FechaResolucion = @FechaResolucion, FechaActualizacion = @Fecha
WHERE Id = @Id AND IdEstado = @IdEstadoActual;";

            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idTicket;
                cmd.Parameters.Add("@IdEstadoActual", SqlDbType.Int).Value = idEstadoActual;
                cmd.Parameters.Add("@IdEstadoNuevo", SqlDbType.Int).Value = idEstadoNuevo;
                cmd.Parameters.Add("@FechaResolucion", SqlDbType.DateTime2).Value = LectorExtensiones.ValorONulo(fechaResolucion);
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
                return cmd.ExecuteNonQuery();
            }
        }


        public int Eliminar(int idTicket)
        {
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand("DELETE FROM dbo.Tickets WHERE Id = @Id;", cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idTicket;
                return cmd.ExecuteNonQuery();
            }
        }

        private static Ticket Mapear(IDataRecord fila)
        {
            return new Ticket
            {
                Id = fila.Entero("Id"),
                Titulo = fila.Texto("Titulo"),
                Descripcion = fila.Texto("Descripcion"),
                Categoria = fila.Texto("Categoria"),
                IdUsuarioSolicitante = fila.Entero("IdUsuarioSolicitante"),
                IdUsuarioAsignado = fila.EnteroNulo("IdUsuarioAsignado"),
                IdEstado = fila.Entero("IdEstado"),
                FechaCreacion = fila.Fecha("FechaCreacion"),
                FechaActualizacion = fila.Fecha("FechaActualizacion"),
                FechaResolucion = fila.FechaNula("FechaResolucion"),
                NombreSolicitante = fila.Texto("NombreSolicitante"),
                NombreAsignado = fila.Texto("NombreAsignado"),
                NombreEstado = fila.Texto("NombreEstado")
            };
        }
    }
}
