
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;

    public interface IImpuestoRepository : IRepository<Impuesto>
    {
       bool ExisteImpuesto(int id);
    }
}
