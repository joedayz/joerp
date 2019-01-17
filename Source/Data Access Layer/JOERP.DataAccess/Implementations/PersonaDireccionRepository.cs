
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class PersonaDireccionRepository : Repository<PersonaDireccion>, IPersonaDireccionRepository
    {
        public IList<PersonaDireccion> GetByIdPersona(int id)
        {
            return Get("usp_Select_PersonaDireccion_ByIdPersona", id);
        }

        public PersonaDireccion PrimeroDireccion(int idPersona)
        {
            return Single("usp_Single_PersonaDireccion_ByIdPersona", idPersona);
        }
    }
}
