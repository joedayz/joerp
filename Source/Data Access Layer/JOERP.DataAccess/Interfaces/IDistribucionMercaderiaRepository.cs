
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;
    using Business.Entity.DTO;

    public interface IDistribucionMercaderiaRepository : IRepository<DistribucionMercaderia>
    {
         bool Insertar(Transaccion transaccion);

         void Eliminar(DistribucionMercaderia distribucion);
    }
}
