
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using Business.Entity;
    using Helpers;
    using Interfaces;

    public class UbigeoRepository : Repository<Comun>, IUbigeoRepository
    {
        public List<Comun> GetAllDepartamentos()
        {
            return Get("usp_Select_UbigeoDepartamentos").ToList();
        }

        public List<Comun> GetAllProvincias(int departamentoId)
        {
            return Get("usp_Select_UbigeoProvincias", departamentoId).ToList();
        }

        public List<Comun> GetAllDistritos(int departamentoid, int provinciaId)
        {
            return Get("usp_Select_UbigeoDistritos", departamentoid, provinciaId).ToList();
        }

        public int GetUbigeoId(int departamentoid, int provinciaId, int distritoid)
        {
            return (int)GetScalar("usp_GetId_Ubigeo", departamentoid, provinciaId, distritoid);
        }

        public Ubigeo Single(int idUbigeo)
        {
            return SingleGeneric<Ubigeo>("usp_Single_Ubigeo", idUbigeo);
        }
    }
}