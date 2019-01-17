
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class PersonaBL : Singleton<PersonaBL>, IPaged<Persona>
    {
        private IPersonaRepository repository = new PersonaRepository();

        public IList<Persona> GetByFuncionFiltro(int idFuncion, string term)
        {
            try
            {
                return repository.GetByFuncionFiltro(idFuncion,term);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
            
        public IList<Persona> GetByFuncion(int idFuncion)
        {
            try
            {
                return repository.GetByFuncion(idFuncion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Persona Single(int idPersona)
        {
            try
            {
                var entidad = repository.Single(idPersona);
                entidad.PersonaDirecciones = PersonaDireccionBL.Instancia.GetByIdPersona(idPersona);
                entidad.PersonaFunciones = PersonaFuncionBL.Instancia.ObtenerFunciones(entidad.IdPersona);
                return entidad;
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

        public IList<Persona> GetPaged(params object[] parameters)
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

        public IList<Persona> Get()
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

        public Persona Add(Persona persona)
        {
            try
            {
                return repository.AddT(persona);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Persona Update(Persona persona)
        {
            try
            {
                return repository.Update(persona);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idPersona)
        {
            try
            {
                repository.DeleteFull(idPersona);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
