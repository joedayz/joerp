
namespace JOERP.DataAccess.Interfaces
{
    using System.Data.Common;
    using Business.Entity;
    
    public interface IProductoKardexRepository : IRepository<ProductoKardex>
    {
        void Insertar(Transaccion transaccion, DbTransaction transaction = null);

        void Actualizar(Transaccion transaccion, DbTransaction transaction = null);

        void Eliminar(int idTransaccion, DbTransaction transaction = null);
    }
}
