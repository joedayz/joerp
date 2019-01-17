
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface ITransaccionDocumentoRepository :  IRepository<TransaccionDocumento>
    {
        IList<TransaccionDocumento> GetByTrasaccion(int idTransaccion);
    }
}
