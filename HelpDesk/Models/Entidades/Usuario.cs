using System;

namespace HelpDesk.Models.Entidades
{
   
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public string Rol { get; set; }
        public DateTime FechaRegistro { get; set; }

        
        public bool Estado { get; set; }

        public bool EsAdministrador
        {
            get { return Rol == RolesSistema.Administrador; }
        }
    }
}
