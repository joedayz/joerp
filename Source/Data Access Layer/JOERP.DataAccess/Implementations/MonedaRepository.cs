
namespace JOERP.DataAccess.Implementations
{
    using Business.Entity;
    using Interfaces;

    public class MonedaRepository : Repository<Moneda>, IMonedaRepository
    {
        public bool ExistMonedaLocal(int idMoneda, int idEmpresa)
        {
            var cantidad = (int) GetScalar("usp_Select_Moneda_ExistMonedaLocal", idMoneda, idEmpresa);
            return cantidad != 0;
        }

        public Moneda GetModenaLocal(int idEmpresa)
        {
            return Single("usp_Select_MonedaLocal_ByIdEmpresa", idEmpresa);
        }
    }
}
