using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HelpDesk.Datos
{
 
    public static class Conexion
    {
        public static SqlConnection Abrir()
        {
            var configuracion = ConfigurationManager.ConnectionStrings["HelpDeskDB"];
            if (configuracion == null)
            {
                throw new InvalidOperationException("No existe la cadena de conexión 'HelpDeskDB' en Web.config.");
            }

            var conexion = new SqlConnection(configuracion.ConnectionString);
            conexion.Open();
            return conexion;
        }
    }


    internal static class LectorExtensiones
    {
        public static int Entero(this IDataRecord fila, string columna)
        {
            return fila.GetInt32(fila.GetOrdinal(columna));
        }

        public static int? EnteroNulo(this IDataRecord fila, string columna)
        {
            int i = fila.GetOrdinal(columna);
            return fila.IsDBNull(i) ? (int?)null : fila.GetInt32(i);
        }

        public static string Texto(this IDataRecord fila, string columna)
        {
            int i = fila.GetOrdinal(columna);
            return fila.IsDBNull(i) ? null : fila.GetString(i);
        }

        public static DateTime Fecha(this IDataRecord fila, string columna)
        {
            return fila.GetDateTime(fila.GetOrdinal(columna));
        }

        public static DateTime? FechaNula(this IDataRecord fila, string columna)
        {
            int i = fila.GetOrdinal(columna);
            return fila.IsDBNull(i) ? (DateTime?)null : fila.GetDateTime(i);
        }

        public static bool Booleano(this IDataRecord fila, string columna)
        {
            return fila.GetBoolean(fila.GetOrdinal(columna));
        }

        /// <summary>Convierte null de C# en DBNull para los parámetros SQL.</summary>
        public static object ValorONulo(object valor)
        {
            return valor ?? DBNull.Value;
        }
    }
}
