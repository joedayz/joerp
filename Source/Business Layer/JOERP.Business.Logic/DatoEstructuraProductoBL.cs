
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class DatoEstructuraProductoBL : Singleton<DatoEstructuraProductoBL>, IPaged<DatoEstructuraProducto>
    {
        private readonly IDatoEstructuraProductoRepository repository = new DatoEstructuraProductoRepository();
        
        public int Count(params object[] parameters)
        {
            try
            {
                return repository.Count(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<DatoEstructuraProducto> GetPaged(params object[] parameters)
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

        public IList<DatoEstructuraProducto> GetDatosByIdProducto(int idProducto)
        {
            try
            {
                var entidad = repository.GetDatosByIdProducto(idProducto);
                foreach (var item in entidad)
                {
                    item.DatoEstructura = DatoEstructuraBL.Instancia.Single(item.IdDatoEstructura);
                }
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
