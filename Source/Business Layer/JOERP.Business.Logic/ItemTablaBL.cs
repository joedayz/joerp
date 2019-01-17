
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class ItemTablaBL : Singleton<ItemTablaBL> , IPaged<ItemTabla>
    {
        private readonly IItemTablaRepository repository = new ItemTablaRepository();

        public int GetMaximoId()
        {
            try
            {
                return repository.GetMaximoId();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<ItemTabla> GetByTabla(int idTabla)
        {
            try
            {
                return repository.GetByTabla(idTabla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ItemTabla Single(int idTabla, int idItemTabla)
        {
            try
            {
                return repository.Single(idTabla, idItemTabla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ItemTabla Add(ItemTabla itemTabla)
        {
            try
            {
                return repository.Add(itemTabla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ItemTabla Update(ItemTabla itemTabla)
        {
            try
            {
                return repository.Update(itemTabla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int itItemTabla, int idTabla)
        {
            try
            {
                repository.Delete(itItemTabla,idTabla);
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
                throw new Exception(ex.Message);
            }
        }

        public IList<ItemTabla> GetPaged(params object[] parameters)
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

        public List<Comun> ItemTablaToList(int idTabla)
        {
            var lista = GetByTabla(idTabla);

            var enumValList = (from l in lista
                                select new Comun
                                {
                                    Id = l.IdItemTabla,
                                    Nombre = l.Nombre
                                })
                .OrderBy(p => p.Nombre)
                .ToList();
            return enumValList;
        }
    }
}
