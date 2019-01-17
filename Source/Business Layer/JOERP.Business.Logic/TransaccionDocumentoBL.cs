
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class TransaccionDocumentoBL : Singleton<TransaccionDocumentoBL>
    {
        private readonly ITransaccionDocumentoRepository repository = new TransaccionDocumentoRepository();

        public IList<TransaccionDocumento> GetByTrasaccion(int idTransaccion)
        {
            try
            {
                return repository.GetByTrasaccion(idTransaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    }
}
