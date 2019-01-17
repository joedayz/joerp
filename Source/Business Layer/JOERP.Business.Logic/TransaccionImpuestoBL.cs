
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class TransaccionImpuestoBL : Singleton<TransaccionImpuestoBL>
    {
        private readonly ITransaccionImpuestoRepository repository = new TransaccionImpuestoRepository();

        public IList<TransaccionImpuesto> GetByTransaccion(int idTransaccion)
        {
            try
            {
                return repository.GetByTransaccion(idTransaccion);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        } 
    }
}
