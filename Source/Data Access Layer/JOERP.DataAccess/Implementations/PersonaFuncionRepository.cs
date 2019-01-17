
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class PersonaFuncionRepository : Repository<PersonaFuncion>, IPersonaFuncionRepository
    {
        public IList<PersonaFuncion> ObtenerFunciones(int idUsuario)
        {
            return Get("usp_PersonaFuncion_ObtenerFunciones", idUsuario);
        }
    }
}
