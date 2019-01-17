
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IPersonaFuncionRepository : IRepository<PersonaFuncion>
    {
        IList<PersonaFuncion> ObtenerFunciones(int idUsuario);
    }
}
