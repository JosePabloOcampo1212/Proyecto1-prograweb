using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HelpDesk.Models;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Datos
{
   
    public class UsuarioRepositorio
    {
        private const string SelectBase = @"
SELECT Id, Nombre, Email, ContrasenaHash, Rol, FechaRegistro, Estado
FROM dbo.Usuarios";

        public Usuario ObtenerPorEmail(string email)
        {
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + " WHERE Email = @Email;", cn))
            {
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
                using (var dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        public Usuario ObtenerPorId(int id)
        {
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + " WHERE Id = @Id;", cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                using (var dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        public List<Usuario> ListarPersonalSoporte()
        {
            var lista = new List<Usuario>();
            using (var cn = Conexion.Abrir())
            using (var cmd = new SqlCommand(SelectBase + " WHERE Rol = @Rol AND Estado = 1 ORDER BY Nombre;", cn))
            {
                cmd.Parameters.Add("@Rol", SqlDbType.NVarChar, 20).Value = RolesSistema.Administrador;
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

        private static Usuario Mapear(IDataRecord fila)
        {
            return new Usuario
            {
                Id = fila.Entero("Id"),
                Nombre = fila.Texto("Nombre"),
                Email = fila.Texto("Email"),
                ContrasenaHash = fila.Texto("ContrasenaHash"),
                Rol = fila.Texto("Rol"),
                FechaRegistro = fila.Fecha("FechaRegistro"),
                Estado = fila.Booleano("Estado")
            };
        }
    }
}
