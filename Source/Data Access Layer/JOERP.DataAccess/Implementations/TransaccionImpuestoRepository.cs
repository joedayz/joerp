
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class TransaccionImpuestoRepository : Repository<TransaccionImpuesto>, ITransaccionImpuestoRepository
    {
        public IList<TransaccionImpuesto> GetByTransaccion(int idTransaccion, DbTransaction transaction = null)
        {
            return Get("usp_Select_TransaccionImpuesto_ByIdTransaccion", idTransaccion, transaction);
        } 

        public void InsertImpuestos(IList<TransaccionImpuesto> impuestos, DbTransaction transaction = null)
        {
            foreach (var transaccionImpuesto in impuestos)
            {
                Add("usp_Insert_TransaccionImpuesto", transaccionImpuesto, transaction);
            }
        }

        public void DeleteByTransaccion(int idTransaccion, DbTransaction transaction)
        {
            var comandoDeleteImpuesto = Database.GetStoredProcCommand("DeleteTransaccionImpuesto");
            Database.AddInParameter(comandoDeleteImpuesto, "pIdTransaccion", DbType.Int32, idTransaccion);
            Database.ExecuteNonQuery(comandoDeleteImpuesto, transaction);
        }
    }
}
