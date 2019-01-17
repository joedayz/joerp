
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class OperacionDocumentoRepository : Repository<OperacionDocumento>, IOperacionDocumentoRepository
    {
        public IList<OperacionDocumento> GetByOperacion(int idOperacion)
        {
            return Get("usp_Select_OperacionDocumento_ByIdOperacion", idOperacion);
        }
    }
}
