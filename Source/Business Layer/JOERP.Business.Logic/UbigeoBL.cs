
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class UbigeoBL : Singleton<UbigeoBL>
    {
        private readonly IUbigeoRepository repository = new UbigeoRepository();

        public Ubigeo Single(int idUbigeo)
        {
            try
            {
                return repository.Single(idUbigeo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Comun> GetAllDepartamentos()
        {
            try
            {
                return repository.GetAllDepartamentos();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Comun> GetAllProvincias(int departamentoId)
        {
            try
            {
                return repository.GetAllProvincias(departamentoId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Comun> GetAllDistritos(int departamentoid, int provinciaId)
        {
            try
            {
                return repository.GetAllDistritos(departamentoid, provinciaId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GetUbigeoId(int departamentoid, int provinciaId, int distritoid)
        {
            try
            {
                return repository.GetUbigeoId(departamentoid, provinciaId, distritoid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}