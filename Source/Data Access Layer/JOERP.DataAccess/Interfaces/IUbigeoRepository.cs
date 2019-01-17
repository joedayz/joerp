
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;
    using Helpers;

    public interface IUbigeoRepository : IRepository<Comun>
    {
        List<Comun> GetAllDepartamentos();

        List<Comun> GetAllProvincias(int departamentoId);

        List<Comun> GetAllDistritos(int departamentoid, int provinciaId);

        int GetUbigeoId(int departamentoid, int provinciaId, int distritoid);

        Ubigeo Single(int idUbigeo);
    }
}