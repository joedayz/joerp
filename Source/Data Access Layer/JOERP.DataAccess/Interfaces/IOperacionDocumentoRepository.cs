
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IOperacionDocumentoRepository : IRepository<OperacionDocumento>
    {
        IList<OperacionDocumento> GetByOperacion(int idOperacion);
    }
}
