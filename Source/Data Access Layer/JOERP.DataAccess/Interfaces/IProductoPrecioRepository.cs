
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface IProductoPrecioRepository : IRepository<ProductoPrecio>
    {
        decimal GetCostoByPresentacion(int idEmpresa, int idSucursal, int idPresentacion, DbTransaction transaction = null);

        IList<ProductoPrecio> GetPreciosBySucursalAndProducto(int idSucursal, int idProducto, DbTransaction transaction = null);

        IList<HistorialPrecioDTO> GetLastPreciosByProductoAndPresentacion(int idEmpresa, int idProducto, int idPresentacion);

        void UpdateCostoGananciaValorPrecioVenta(ProductoPrecio productoPrecio, DbTransaction transaction);

        void UpdateCostoProductoPrecio(ProductoPrecio productoPrecio, DbTransaction transaction);
    }
}
