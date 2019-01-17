
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class EmpleadoBL : Singleton<EmpleadoBL>, IPaged<Empleado>
    {
        private IEmpleadoRepository repository = new EmpleadoRepository();

        public IList<Empleado> GetAll()
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

        public IList<Empleado> GetByIdCargo(int idCargo)
        {
            try
            {
                var entidades =repository.GetByIdCargo(idCargo);
                foreach (var item in entidades)
                {
                    item.Persona = PersonaBL.Instancia.Single(item.IdEmpleado);
                }
                return entidades;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Empleado Single(int id)
        {
            try
            {
                var empleado = repository.Single(id);
                empleado.Persona = PersonaBL.Instancia.Single(empleado.IdEmpleado);
                empleado.Cargo = CargoBL.Instancia.Single(empleado.IdCargo);
                empleado.Persona.PersonaFunciones = PersonaFuncionBL.Instancia.ObtenerFunciones(empleado.Persona.IdPersona);
                return empleado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int MaxId()
        {
            try
            {
                return repository.MaxId();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Empleado Add(Empleado entity)
        {
            try
            {
                return repository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Empleado Update(Empleado entity)
        {
            try
            {
                return repository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                repository.Delete(id);
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
                return  repository.Count(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Empleado> GetPaged(params object[] parameters)
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
    }
}
