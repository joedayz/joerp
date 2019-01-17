
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IPersonaRepository : IRepository<Persona>
    {
        Persona AddT(Persona entity);
        List<Persona> GetContactos(int idPersonaPadre);
        bool Insertar(Persona persona, List<Persona> lista);
        bool Actualizar(Persona persona, List<Persona> lista);
        IList<Persona> GetByFuncion(int idFuncion);
        IList<Persona> GetByFuncionFiltro(int idFuncion, string term);
        void DeleteFull(int idPersona);
    }
}
