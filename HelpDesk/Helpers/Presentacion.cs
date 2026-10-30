using System;
using HelpDesk.Models;

namespace HelpDesk.Helpers
{

    public static class Formato
    {
        public const string FechaHora = "dd/MM/yyyy HH:mm";

        public static string Fecha(DateTime fecha)
        {
            return fecha.ToString(FechaHora);
        }

        public static string Fecha(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString(FechaHora) : null;
        }

        public static string Horas(double horas)
        {
            return horas.ToString("0.0") + " h";
        }
    }

    public static class EstiloEstado
    {
        public static string Clase(int idEstado)
        {
            switch (idEstado)
            {
                case Estados.Abierto: return "text-bg-primary";
                case Estados.EnProceso: return "text-bg-warning";
                case Estados.Resuelto: return "text-bg-success";
                case Estados.Cerrado: return "text-bg-secondary";
                default: return "text-bg-light";
            }
        }
    }
}
