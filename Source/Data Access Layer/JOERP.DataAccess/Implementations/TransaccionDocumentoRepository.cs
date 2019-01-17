
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class TransaccionDocumentoRepository : Repository<TransaccionDocumento>, ITransaccionDocumentoRepository
    {
        public IList<TransaccionDocumento> GetByTrasaccion(int idTransaccion)
        {
            return Get("usp_Select_TransaccionDocumento_ByTransaccion", idTransaccion);
        }
    }
}
