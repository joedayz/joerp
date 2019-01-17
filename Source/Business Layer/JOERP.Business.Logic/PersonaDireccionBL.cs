
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class PersonaDireccionBL : Singleton <PersonaDireccionBL>
    {
        private readonly IPersonaDireccionRepository repository = new PersonaDireccionRepository();

        public PersonaDireccion PrimeroDireccion(int idPersona)
        {
            try
            {
                var entidad = repository.PrimeroDireccion(idPersona);
                if(entidad!=null)
                    entidad.Ubigeo = UbigeoBL.Instancia.Single(entidad.IdUbigeo);
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PersonaDireccion SingleD(int id)
        {
            try
            {
                return repository.Single(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<PersonaDireccion> GetByIdPersona(int idPersona)
        {
            try
            {
                var entidad = repository.GetByIdPersona(idPersona);
                foreach (var item in entidad)
                {
                    item.Ubigeo = UbigeoBL.Instancia.Single(item.IdUbigeo);
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
