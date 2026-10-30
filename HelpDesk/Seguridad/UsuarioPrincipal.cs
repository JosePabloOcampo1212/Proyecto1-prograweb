using System;
using System.Security.Principal;
using System.Web.Security;
using HelpDesk.Models;
using HelpDesk.Models.Entidades;

namespace HelpDesk.Seguridad
{
   
    public class UsuarioPrincipal : IPrincipal
    {
        public UsuarioPrincipal(IIdentity identidad, int id, string nombre, string rol)
        {
            Identity = identidad;
            Id = id;
            Nombre = nombre;
            Rol = rol;
        }

        public IIdentity Identity { get; private set; }
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Rol { get; private set; }

        public bool EsAdministrador
        {
            get { return Rol == RolesSistema.Administrador; }
        }

        public bool IsInRole(string role)
        {
            return string.Equals(Rol, role, StringComparison.OrdinalIgnoreCase);
        }

      
        public static string CrearDatosCookie(Usuario usuario)
        {
            return usuario.Id + "|" + usuario.Rol + "|" + usuario.Nombre;
        }

        public static UsuarioPrincipal DesdeIdentidad(FormsIdentity identidad)
        {
            if (identidad == null || identidad.Ticket == null || string.IsNullOrEmpty(identidad.Ticket.UserData))
            {
                return null;
            }

            string[] partes = identidad.Ticket.UserData.Split(new[] { '|' }, 3);
            int id;
            if (partes.Length != 3 || !int.TryParse(partes[0], out id))
            {
                return null;
            }

            return new UsuarioPrincipal(identidad, id, partes[2], partes[1]);
        }
    }
}
