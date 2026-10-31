using System;

namespace HelpDesk.Negocio
{
    /// <summary>
    /// Se lanza cuando una operación incumple una regla del negocio
    /// (por ejemplo, comentar un ticket cerrado). Se responde con HTTP 400.
    /// </summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }

    /// <summary>
    /// Se lanza cuando la persona no tiene permiso sobre el recurso
    /// (por ejemplo, un empleado que intenta ver un ticket ajeno). Se responde con HTTP 403.
    /// </summary>
    public class AccesoDenegadoException : Exception
    {
        public AccesoDenegadoException(string mensaje) : base(mensaje) { }
    }

    /// <summary>
    /// Se lanza cuando el recurso solicitado no existe. Se responde con HTTP 404.
    /// </summary>
    public class RecursoNoEncontradoException : Exception
    {
        public RecursoNoEncontradoException(string mensaje) : base(mensaje) { }
    }
}
