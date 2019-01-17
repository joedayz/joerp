
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Business.Entity;

    public interface IKardexRepository 
    {
        List<Kardex> SelectKardexValorizado(int idEmpresa, int idSucursal, int idProducto, int idPresentacion,
                DateTime? fechaInicio, DateTime? fechaFin, out decimal cantidadInicial, out decimal costoTotalInicial);
    }
}
