namespace HelpDesk.Models
{
   
    public static class RolesSistema
    {
        public const string Usuario = "Usuario";
        public const string Administrador = "Administrador";
    }

  
    public static class Estados
    {
        public const int Abierto = 1;
        public const int EnProceso = 2;
        public const int Resuelto = 3;
        public const int Cerrado = 4;
    }


    public static class Categorias
    {
        public static readonly string[] Lista = { "Hardware", "Software", "Red", "Accesos", "Otro" };
    }
}
