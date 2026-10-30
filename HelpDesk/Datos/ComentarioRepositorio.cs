using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Datos
{

    public class ComentarioRepositorio
    {
        private const string SelectBase = @"
SELECT c.Id, c.IdTicket, c.IdUsuario, c.Texto, c.Fecha,
       u.Nombre AS NombreUsuario, u.Rol AS RolUsuario
FROM dbo.Comentarios c
INNER JOIN dbo.Usuarios u ON u.Id = c.IdUsuario";

        public List<Comentario> ListarPorTicket(int idTicket)
        {
            var lista = new List<Comentario>();
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + " WHERE c.IdTicket = @IdTicket ORDER BY c.Fecha, c.Id;", cn))
            {
                cmd.Parameters.Add("@IdTicket", SqlDbType.Int).Value = idTicket;
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

        public Comentario ObtenerPorId(int id)
        {
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + " WHERE c.Id = @Id;", cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                using (var dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

     
        public int Insertar(Comentario comentario)
        {
            using (var cn = Conexion.Abrir())
            using (var transaccion = cn.BeginTransaction())
            {
                try
                {
                    int id;
                    using (var cmd = new SqlCommand(@"
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @IdUsuario, @Texto, @Fecha);
SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, transaccion))
                    {
                        cmd.Parameters.Add("@IdTicket", SqlDbType.Int).Value = comentario.IdTicket;
                        cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = comentario.IdUsuario;
                        cmd.Parameters.Add("@Texto", SqlDbType.NVarChar, 1000).Value = comentario.Texto;
                        cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = comentario.Fecha;
                        id = (int)cmd.ExecuteScalar();
                    }

                    using (var cmd = new SqlCommand(
                        "UPDATE dbo.Tickets SET FechaActualizacion = @Fecha WHERE Id = @IdTicket;", cn, transaccion))
                    {
                        cmd.Parameters.Add("@IdTicket", SqlDbType.Int).Value = comentario.IdTicket;
                        cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = comentario.Fecha;
                        cmd.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    return id;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        private static Comentario Mapear(IDataRecord fila)
        {
            return new Comentario
            {
                Id = fila.Entero("Id"),
                IdTicket = fila.Entero("IdTicket"),
                IdUsuario = fila.Entero("IdUsuario"),
                Texto = fila.Texto("Texto"),
                Fecha = fila.Fecha("Fecha"),
                NombreUsuario = fila.Texto("NombreUsuario"),
                RolUsuario = fila.Texto("RolUsuario")
            };
        }
    }
}
