
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class PersonaFuncionBL : Singleton<PersonaFuncionBL>
    {
        private readonly IPersonaFuncionRepository repository = new PersonaFuncionRepository();

        public IList<PersonaFuncion> ObtenerFunciones(int idUsuario)
        {
            try
            {
                return repository.ObtenerFunciones(idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
