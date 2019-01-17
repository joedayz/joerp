
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class CargoBL : Singleton<CargoBL>, IPaged<Cargo>
    {
        private readonly ICargoRepository repository = new CargoRepository();

        public IList<Cargo> Get()
        {
            try
            {
                return repository.Get();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Cargo Single(int idCargo)
        {
            try
            {
                return repository.Single(idCargo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public Cargo Add(Cargo cargo)
        {
            try
            {
                return repository.Add(cargo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Cargo Update(Cargo cargo)
        {
            try
            {
                return repository.Update(cargo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idCargo)
        {
            try
            {
                repository.Delete(idCargo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int Count(params object[] parameters)
        {
            try
            {
                return repository.Count(parameters);
            }
            catch (Exception ex)
            {
                throw  new Exception(ex.Message);
            }
        }

        public IList<Cargo> GetPaged(params object[] parameters)
        {
            try
            {
                return repository.GetPaged(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Cargo> GetByEmpresa(int idEmpresa)
        {
            try
            {
                return repository.GetByEmpresa(idEmpresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
