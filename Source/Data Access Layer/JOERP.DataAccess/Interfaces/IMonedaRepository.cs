
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;

    public interface IMonedaRepository : IRepository<Moneda>
    {
        bool ExistMonedaLocal(int idMoneda, int idEmpresa);

        Moneda GetModenaLocal(int idEmpresa);
    }
}
