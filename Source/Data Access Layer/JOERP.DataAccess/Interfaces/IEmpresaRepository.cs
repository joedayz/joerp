
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;

    public interface IEmpresaRepository : IRepository<Empresa>
    {
        Empresa SingleByRazonSocial(string razonSocial);
    }
}
