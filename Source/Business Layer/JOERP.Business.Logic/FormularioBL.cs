
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class FormularioBL : Singleton<FormularioBL>, IPaged<Formulario>
    {
        private readonly IFormularioRepository repository = new FormularioRepository();

        public List<int> Modulos(int idUsuario)
        {
            try
            {
                return repository.Modulos(idUsuario).Select(item=>item.IdModulo).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Formulario> Formularios(int idUsuario, int idModulo)
        {
            try
            {
                return Formularios(idUsuario, idModulo, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Formulario> Formularios(int idUsuario, int idModulo, int? idParent)
        {
            try
            {
                var formularios = repository.Formularios(idUsuario, idModulo, idParent);

                foreach (var formulario in formularios)
                {
                    formulario.Formularios = Formularios(idUsuario, idModulo, formulario.IdFormulario);
                }
                return formularios;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Formulario> FormulariosLista(int idModulo, int? idParent)
        {
            try
            {
                return repository.Formularios(idModulo, idParent);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Formulario> Get()
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

        public Formulario Single(int idFormulario)
        {
            try
            {
                return repository.Single(idFormulario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Formulario Add(Formulario formulario)
        {
            try
            {
                return repository.Add(formulario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Formulario Update(Formulario formulario)
        {
            try
            {
                return repository.Update(formulario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idFormulario)
        {
            try
            {
                repository.Delete(idFormulario);
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

        public IList<Formulario> GetPaged(params object[] parameters)
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
