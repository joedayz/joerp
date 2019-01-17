
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;
    
    public interface ITransaccionImpuestoRepository : IRepository<TransaccionImpuesto>
    {
        IList<TransaccionImpuesto> GetByTransaccion(int idTransaccion, DbTransaction transaction = null);

        void InsertImpuestos(IList<TransaccionImpuesto> impuestos, DbTransaction transaction = null);

        void DeleteByTransaccion(int idTransaccion, DbTransaction transaction);
    }
}
