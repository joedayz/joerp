
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IPersonaDireccionRepository : IRepository<PersonaDireccion>
    {
        IList<PersonaDireccion> GetByIdPersona(int id);
        PersonaDireccion PrimeroDireccion(int idPersona);
    }
}
