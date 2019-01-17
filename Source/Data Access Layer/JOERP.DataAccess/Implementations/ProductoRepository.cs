
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Business.Entity;
    using Interfaces;

    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        public int MaxId()
        {
            return (int)GetScalar("usp_MaximoId_Producto");
        }
        
        public new Producto Add (Producto producto)
        { 
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Producto

                        var comandoInsertarProducto = Database.GetStoredProcCommand("usp_Insert_Producto");
                        Database.AddOutParameter(comandoInsertarProducto, "IdProducto", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarProducto, "Codigo", DbType.String, producto.Codigo);
                        Database.AddInParameter(comandoInsertarProducto, "Nombre", DbType.String, producto.Nombre);
                        Database.AddInParameter(comandoInsertarProducto, "Descripcion", DbType.String, producto.Descripcion);
                        Database.AddInParameter(comandoInsertarProducto, "CodigoBarra", DbType.String, producto.CodigoBarra);
                        Database.AddInParameter(comandoInsertarProducto, "Estado", DbType.Int32, producto.Estado);
                        Database.AddInParameter(comandoInsertarProducto, "IdEmpresa", DbType.Int32, producto.IdEmpresa);
                        Database.AddInParameter(comandoInsertarProducto, "StockMaximo", DbType.Decimal, producto.StockMaximo);
                        Database.AddInParameter(comandoInsertarProducto, "StockMinimo", DbType.Decimal, producto.StockMinimo);
                        Database.AddInParameter(comandoInsertarProducto, "EsAfecto", DbType.Boolean, producto.EsAfecto);
                        Database.AddInParameter(comandoInsertarProducto, "CodigoAlterno", DbType.String, producto.CodigoAlterno);
                        Database.AddInParameter(comandoInsertarProducto, "EsExonerado", DbType.Int32, producto.EsExonerado);
                        Database.AddInParameter(comandoInsertarProducto, "TipoProducto", DbType.Int32, producto.TipoProducto);
                        Database.AddInParameter(comandoInsertarProducto, "DescripcionLarga", DbType.String, producto.DescripcionLarga);
                        Database.AddInParameter(comandoInsertarProducto, "TipoClasificacion", DbType.Int32, producto.TipoClasificacion);
                        Database.AddInParameter(comandoInsertarProducto, "Imagen", DbType.Binary, producto.Imagen);
                        Database.AddInParameter(comandoInsertarProducto, "FechaCreacion", DbType.DateTime, producto.FechaCreacion);
                        Database.AddInParameter(comandoInsertarProducto, "FechaModificacion", DbType.DateTime, producto.FechaModificacion);
                        Database.AddInParameter(comandoInsertarProducto, "UsuarioCreacion", DbType.Int32, producto.UsuarioCreacion);
                        Database.AddInParameter(comandoInsertarProducto, "UsuarioModificacion", DbType.Int32, producto.UsuarioModificacion);

                        Database.ExecuteNonQuery(comandoInsertarProducto, transaction);
                        producto.IdProducto= Convert.ToInt32(Database.GetParameterValue(comandoInsertarProducto, "IdProducto"));

                        #endregion Producto

                        #region Presentaciones

                        var presentaciones = producto.Presentacion;
                        var comandoInsertarPresentacion = Database.GetStoredProcCommand("usp_Insert_Presentacion");

                        Database.AddInParameter(comandoInsertarPresentacion, "IdProducto", DbType.Int32);
                        Database.AddOutParameter(comandoInsertarPresentacion, "IdPresentacion", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarPresentacion, "IdUnidadMedida", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPresentacion, "Nombre", DbType.String);
                        Database.AddInParameter(comandoInsertarPresentacion, "Peso", DbType.Decimal);
                        Database.AddInParameter(comandoInsertarPresentacion, "Equivalencia", DbType.Decimal);
                        Database.AddInParameter(comandoInsertarPresentacion, "EsBase", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPresentacion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPresentacion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPresentacion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPresentacion, "UsuarioModificacion", DbType.Int32);

                        foreach (var presentacion in presentaciones)
                        {
                            Database.SetParameterValue(comandoInsertarPresentacion, "IdPresentacion", presentacion.IdPresentacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "IdProducto", producto.IdProducto);
                            Database.SetParameterValue(comandoInsertarPresentacion, "IdUnidadMedida", presentacion.IdUnidadMedida);
                            Database.SetParameterValue(comandoInsertarPresentacion, "Nombre", presentacion.Nombre);
                            Database.SetParameterValue(comandoInsertarPresentacion, "Peso", presentacion.Peso);
                            Database.SetParameterValue(comandoInsertarPresentacion, "Equivalencia", presentacion.Equivalencia);
                            Database.SetParameterValue(comandoInsertarPresentacion, "EsBase", presentacion.EsBase);
                            Database.SetParameterValue(comandoInsertarPresentacion, "FechaCreacion", presentacion.FechaCreacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "FechaModificacion", presentacion.FechaModificacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "UsuarioCreacion", presentacion.UsuarioCreacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "UsuarioModificacion", presentacion.UsuarioModificacion);

                            Database.ExecuteNonQuery(comandoInsertarPresentacion, transaction);
                        }

                        #endregion Presentaciones

                        #region Estructura producto

                        var datoEstructuraProductoRepository = new DatoEstructuraProductoRepository();

                        foreach (var datoEstructuraProducto in producto.DatoEstructuraProducto)
                        {
                            datoEstructuraProducto.IdProducto = producto.IdProducto;
                            datoEstructuraProductoRepository.Add(datoEstructuraProducto, transaction);
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            return producto;
        }

        public new Producto Update(Producto producto)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Presentaciones
                        
                        var comandoBorrarPresentaciones = Database.GetStoredProcCommand("usp_Delete_Presentacion");
                        Database.AddInParameter(comandoBorrarPresentaciones,"IdProducto",DbType.Int32);
                        Database.SetParameterValue(comandoBorrarPresentaciones,"IdProducto",producto.IdProducto);
                        Database.ExecuteNonQuery(comandoBorrarPresentaciones,transaction);
                        
                        var presentaciones = producto.Presentacion;
                        
                        var comandoInsertarPresentacion = Database.GetStoredProcCommand("usp_Insert_Presentacion");
                        Database.AddInParameter(comandoInsertarPresentacion, "IdProducto", DbType.Int32);
                        Database.AddOutParameter(comandoInsertarPresentacion, "IdPresentacion", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarPresentacion, "IdUnidadMedida", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPresentacion, "Nombre", DbType.String);
                        Database.AddInParameter(comandoInsertarPresentacion, "Peso", DbType.Decimal);
                        Database.AddInParameter(comandoInsertarPresentacion, "Equivalencia", DbType.Decimal);
                        Database.AddInParameter(comandoInsertarPresentacion, "EsBase", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPresentacion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPresentacion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPresentacion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPresentacion, "UsuarioModificacion", DbType.Int32);

                        foreach (var presentacion in presentaciones)
                        {
                            Database.SetParameterValue(comandoInsertarPresentacion, "IdPresentacion", presentacion.IdPresentacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "IdProducto", presentacion.IdProducto);
                            Database.SetParameterValue(comandoInsertarPresentacion, "IdUnidadMedida", presentacion.IdUnidadMedida);
                            Database.SetParameterValue(comandoInsertarPresentacion, "Nombre", presentacion.Nombre);
                            Database.SetParameterValue(comandoInsertarPresentacion, "Peso", presentacion.Peso);
                            Database.SetParameterValue(comandoInsertarPresentacion, "Equivalencia", presentacion.Equivalencia);
                            Database.SetParameterValue(comandoInsertarPresentacion, "EsBase", presentacion.EsBase);
                            Database.SetParameterValue(comandoInsertarPresentacion, "FechaCreacion", presentacion.FechaCreacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "FechaModificacion", presentacion.FechaModificacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "UsuarioCreacion", presentacion.UsuarioCreacion);
                            Database.SetParameterValue(comandoInsertarPresentacion, "UsuarioModificacion", presentacion.UsuarioModificacion);

                            Database.ExecuteNonQuery(comandoInsertarPresentacion, transaction);
                        }

                        #endregion Presentaciones

                        #region Producto

                        var comandoActualizarProducto = Database.GetStoredProcCommand("usp_Update_Producto");
                        Database.AddInParameter(comandoActualizarProducto, "IdProducto", DbType.Int32, producto.IdProducto);
                        Database.AddInParameter(comandoActualizarProducto, "Codigo", DbType.String, producto.Codigo);
                        Database.AddInParameter(comandoActualizarProducto, "Nombre", DbType.String, producto.Nombre);
                        Database.AddInParameter(comandoActualizarProducto, "Descripcion", DbType.String, producto.Descripcion);
                        Database.AddInParameter(comandoActualizarProducto, "CodigoBarra", DbType.String, producto.CodigoBarra);
                        Database.AddInParameter(comandoActualizarProducto, "Estado", DbType.Int32, producto.Estado);
                        Database.AddInParameter(comandoActualizarProducto, "IdEmpresa", DbType.Int32, producto.IdEmpresa);
                        Database.AddInParameter(comandoActualizarProducto, "StockMaximo", DbType.Decimal, producto.StockMaximo);
                        Database.AddInParameter(comandoActualizarProducto, "StockMinimo", DbType.Decimal, producto.StockMinimo);
                        Database.AddInParameter(comandoActualizarProducto, "EsAfecto", DbType.Boolean, producto.EsAfecto);
                        Database.AddInParameter(comandoActualizarProducto, "CodigoAlterno", DbType.String, producto.CodigoAlterno);
                        Database.AddInParameter(comandoActualizarProducto, "EsExonerado", DbType.Int32, producto.EsExonerado);
                        Database.AddInParameter(comandoActualizarProducto, "TipoProducto", DbType.Int32, producto.TipoProducto);
                        Database.AddInParameter(comandoActualizarProducto, "DescripcionLarga", DbType.String, producto.DescripcionLarga);
                        Database.AddInParameter(comandoActualizarProducto, "TipoClasificacion", DbType.Int32, producto.TipoClasificacion);
                        Database.AddInParameter(comandoActualizarProducto, "Imagen", DbType.Binary, producto.Imagen);
                        Database.AddInParameter(comandoActualizarProducto, "FechaCreacion", DbType.DateTime, producto.FechaCreacion);
                        Database.AddInParameter(comandoActualizarProducto, "FechaModificacion", DbType.DateTime, producto.FechaModificacion);
                        Database.AddInParameter(comandoActualizarProducto, "UsuarioCreacion", DbType.Int32, producto.UsuarioCreacion);
                        Database.AddInParameter(comandoActualizarProducto, "UsuarioModificacion", DbType.Int32, producto.UsuarioModificacion);
                       
                        Database.ExecuteNonQuery(comandoActualizarProducto, transaction);

                        #endregion Producto

                        #region Estructura producto

                        var comandoEliminarDatoEstructuraProducto = Database.GetStoredProcCommand("usp_Delete_DatoEstructuraProducto");
                        Database.AddInParameter(comandoEliminarDatoEstructuraProducto, "IdProducto", DbType.Int32, producto.IdProducto);
                        Database.ExecuteNonQuery(comandoEliminarDatoEstructuraProducto, transaction);

                        var datoEstructuraProductoRepository = new DatoEstructuraProductoRepository();

                        foreach (var datoEstructuraProducto in producto.DatoEstructuraProducto)
                        {
                            datoEstructuraProducto.IdProducto = producto.IdProducto;
                            datoEstructuraProductoRepository.Add(datoEstructuraProducto, transaction);
                        }

                        #endregion
 
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            return producto;
           
        }

        public int Delete(Producto entity)
        {
            return 1;
        }

        public bool ExisteCodigo(string codigo, int id, int empresaId)
        {
            var estado = true;
            var cantidad = (int)GetScalar("usp_ExisteCodigo_Producto", codigo, id, empresaId);
            if (cantidad == 0)
               estado = false;
            return estado;
        }

        public List<Inventario> GetInventario(int idAlmacen)
        {
            var registros = new List<Inventario>();

            var comandoSelectInventario = Database.GetStoredProcCommand("GetInventario");
            Database.AddInParameter(comandoSelectInventario, "pIdAlmacen", DbType.Int32, idAlmacen);
            using (var dr = Database.ExecuteReader(comandoSelectInventario))
            {
                while (dr.Read())
                {
                    registros.Add(new Inventario
                    {
                        Codigo = dr.GetString(dr.GetOrdinal("Codigo")),
                        CodigoBarra = dr.GetString(dr.GetOrdinal("CodigoBarra")),
                        FechaVencimiento = dr.IsDBNull(dr.GetOrdinal("FechaVencimiento")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("FechaVencimiento")),
                        Lote = dr.IsDBNull(dr.GetOrdinal("Lote")) ? string.Empty : dr.GetString(dr.GetOrdinal("Lote")),
                        Serie = dr.GetString(dr.GetOrdinal("Serie")),
                        Producto = dr.GetString(dr.GetOrdinal("Producto")),
                        Presentacion = dr.GetString(dr.GetOrdinal("Presentacion")),
                        StockSistema = dr.GetDecimal(dr.GetOrdinal("StockFisico")),
                        CostoSistema = dr.GetDecimal(dr.GetOrdinal("Costo")),
                        IdProductoStock = dr.GetInt32(dr.GetOrdinal("IdProductoStock"))
                    });
                }
            }

            return registros;
        }

        public List<Inventario> SelectInventarioByParameters(int idEmpresa, int idSucursal, int idAlmacen, int idProveedor, int tipo, string texto, string estructura)
        {
            var registros = new List<Inventario>();
            string codigo = string.Empty, nombre = string.Empty, codigoAlterno = string.Empty;

            switch (tipo)
            {
                case 0: codigo = texto;
                    break;
                case 1: nombre = texto;
                    break;
                case 2: codigoAlterno = texto;
                    break;
            }

            #region Get Inventario

            var comandoSelectInventario = Database.GetStoredProcCommand("SelectInventarioByParameters");
            Database.AddInParameter(comandoSelectInventario, "pIdEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(comandoSelectInventario, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(comandoSelectInventario, "pIdAlmacen", DbType.Int32, idAlmacen);
            Database.AddInParameter(comandoSelectInventario, "pIdProveedor", DbType.Int32, idProveedor);
            Database.AddInParameter(comandoSelectInventario, "pCodigo", DbType.String, codigo);
            Database.AddInParameter(comandoSelectInventario, "pNombre", DbType.String, nombre);
            Database.AddInParameter(comandoSelectInventario, "pCodigoAlterno", DbType.String, codigoAlterno);
            Database.AddInParameter(comandoSelectInventario, "pEstructura", DbType.String, estructura);

            using (var dr = Database.ExecuteReader(comandoSelectInventario))
            {
                while (dr.Read())
                {
                    var stock = dr.IsDBNull(dr.GetOrdinal("Stock")) ? default(decimal) : dr.GetDecimal(dr.GetOrdinal("Stock"));
                    var costo = dr.IsDBNull(dr.GetOrdinal("Costo")) ? default(decimal) : dr.GetDecimal(dr.GetOrdinal("Costo"));

                    registros.Add(new Inventario
                    {
                        IdProducto = dr.GetInt32(dr.GetOrdinal("IdProducto")),
                        Codigo = dr.GetString(dr.GetOrdinal("Codigo")),
                        CodigoBarra = dr.GetString(dr.GetOrdinal("CodigoBarra")),
                        Producto = dr.GetString(dr.GetOrdinal("Nombre")),
                        IdPresentacion = dr.GetInt32(dr.GetOrdinal("IdPresentacion")),
                        Presentacion = dr.GetString(dr.GetOrdinal("Presentacion")),
                        Equivalencia = dr.GetDecimal(dr.GetOrdinal("Equivalencia")),
                        StockSistema = stock,
                        CostoSistema = costo
                    });
                }
            }

            #endregion Get Inventario

            return registros;
        }

        public IList<Producto> GetByTipoProducto(int idEmpresa, int tipoProducto)
        {
            return Get("usp_Select_Producto_ByTipoProducto", idEmpresa, tipoProducto);
        }

        public IList<Producto> GetComponentes(string codigo)
        {
            return Get("usp_Select_Producto_GetComponentes", codigo);
        }

        public IList<Producto> GetProductosByCodigoAndTipo(string codigo, int tipo)
        {
            return Get("usp_Select_Producto_GetByCodigoAndTipo", codigo, tipo);
        }

        public IList<Producto> GetByCodigo(int idEmpresa, string codigo)
        {
            return Get("usp_Select_Producto_GetByCodigo", idEmpresa, codigo);
        }

        public IList<Producto> GetByCodigoAlterno(int idEmpresa, string codigo)
        {
            return Get("usp_Select_Producto_GetByCodigoAlterno", idEmpresa, codigo);
        }

        public IList<Producto> GetProductoByFiltro(int idEmpresa, string filtro)
        {
            return Get("usp_Select_Producto_GetByFiltro", idEmpresa, filtro);
        }

        public Producto GetByCodigo(int idEmpresa, int tipoProducto, string codigo)
        {
            return Single("usp_Select_Producto_GetByCodigoT", idEmpresa, tipoProducto, codigo);
        }

        public IList<Producto> GetByNombre(string nombre)
        {
            return Get("usp_Select_Producto_GetByNombre", nombre);
        }

        public void GuardarComponentes(List<ProductoComponente> componentes)
        {
            if (componentes.Count == 0) return;

            Delete("usp_Delete_ProductoComponente_ByProducto", componentes.FirstOrDefault().IdProducto);

            foreach (var componente in componentes)
            {
                AddGeneric("usp_Insert_ProductoComponente", componente);
            }
        }

        public IList<ProductoStock> GetLotesProductoByAlmacen(int idPresentacion, int idAlmacen)
        {
            return GetGeneric<ProductoStock>("usp_Select_LoteByProductoAlmacen", idPresentacion, idAlmacen);
        }
    }
}