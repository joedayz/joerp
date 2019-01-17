
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Business.Entity.DTO;
    using Interfaces;
    using Helpers;

    public class ProductoPrecioRepository : Repository<ProductoPrecio>, IProductoPrecioRepository
    {
        private readonly IListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository();

        public decimal GetCostoByPresentacion(int idEmpresa, int idSucursal, int idPresentacion, DbTransaction transaction = null)
        {
            var costo = 0m;
            var listaPrecioBase = listaPrecioRepository.GetListaBase(idEmpresa);

            if (listaPrecioBase == null)
            {
                throw new Exception("No existe una lista de precio base para la empresa en sesión.");
            }

            if (transaction == null)
            {
                var productoPrecio = Single("usp_Select_ProductoPrecio_BySucursalListaPrecioAndPresentacion", idSucursal, listaPrecioBase.IdListaPrecio, idPresentacion);

                if (productoPrecio != null)
                {
                    costo = productoPrecio.Costo;
                }
            }
            else
            {
                var comandoProductoPrecio = Database.GetStoredProcCommand("SelectCostoByPresentacionAndListaPrecio");
                Database.AddInParameter(comandoProductoPrecio, "pIdSucursal", DbType.Int32, idSucursal);
                Database.AddInParameter(comandoProductoPrecio, "pIdPresentacion", DbType.Int32, idPresentacion);
                Database.AddInParameter(comandoProductoPrecio, "pIdListaPrecio", DbType.Int32, listaPrecioBase.IdListaPrecio);

                using (var dr = Database.ExecuteReader(comandoProductoPrecio, transaction))
                {
                    if (dr.Read())
                    {
                        costo = dr.GetDecimal(dr.GetOrdinal("Costo"));
                    }
                }
            }

            return costo.Redondear();
        }

        public IList<ProductoPrecio> GetPreciosBySucursalAndProducto(int idSucursal, int idProducto, DbTransaction transaction = null)
        {
            var precios = transaction == null
                               ? Get("usp_Select_ProductoPrecio_BySucursalAndProducto", idSucursal, idProducto)
                               : Get("usp_Select_ProductoPrecio_BySucursalAndProducto", transaction, idSucursal, idProducto);
            return precios;
        }

        public IList<HistorialPrecioDTO> GetLastPreciosByProductoAndPresentacion(int idEmpresa, int idProducto, int idPresentacion)
        {
            var precios = new List<HistorialPrecioDTO>();

            var comandoProductoPrecio = Database.GetStoredProcCommand("SelectLastPreciosByProductoAndPresentacion");
            Database.AddInParameter(comandoProductoPrecio, "pIdEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(comandoProductoPrecio, "pIdProducto", DbType.Int32, idProducto);
            Database.AddInParameter(comandoProductoPrecio, "pIdPresentacion", DbType.Int32, idPresentacion);

            using (var dr = Database.ExecuteReader(comandoProductoPrecio))
            {
                while (dr.Read())
                {
                    precios.Add(
                        new HistorialPrecioDTO
                        {
                            IdTransaccion = dr.GetInt32(dr.GetOrdinal("IdTransaccion")),
                            FechaRegistro = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                            Documento = dr.GetString(dr.GetOrdinal("Documento")),
                            Proveedor = dr.GetString(dr.GetOrdinal("Proveedor")),
                            Cantidad = dr.GetDecimal(dr.GetOrdinal("Cantidad")),
                            Descuento = dr.GetDecimal(dr.GetOrdinal("MontoDescuento")),
                            Precio = dr.GetDecimal(dr.GetOrdinal("PrecioBase"))
                        });
                }
            }

            return precios;
        }

        public void UpdateCostoGananciaValorPrecioVenta(ProductoPrecio productoPrecio, DbTransaction transaction)
        {
            Update("Update_CostoGananciaValorPrecioVenta", productoPrecio, transaction);
        }

        public void UpdateCostoProductoPrecio(ProductoPrecio productoPrecio, DbTransaction transaction)
        {
            Update("Update_CostoProductoPrecio", productoPrecio, transaction);
        }
    }
}
