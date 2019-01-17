
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Business.Entity;
    using Interfaces;
    using Microsoft.Practices.EnterpriseLibrary.Data;

    public class KardexRepository : IKardexRepository
    {
        Database Database = DatabaseFactory.CreateDatabase();

        public List<Kardex> SelectKardexValorizado(int idEmpresa, int idSucursal, int idProducto, int idPresentacion,
                DateTime? fechaInicio, DateTime? fechaFin, out decimal cantidadInicial, out decimal costoTotalInicial)
        {
            var listaKardex = new List<Kardex>();

            var comandoKardex = Database.GetStoredProcCommand("SelectKardexValorizado");
            Database.AddInParameter(comandoKardex, "pIdEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(comandoKardex, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(comandoKardex, "pIdProducto", DbType.Int32, idProducto);
            Database.AddInParameter(comandoKardex, "pIdPresentacion", DbType.Int32, idPresentacion);
            Database.AddInParameter(comandoKardex, "pFechaInicio", DbType.DateTime, fechaInicio);
            Database.AddInParameter(comandoKardex, "pFechaFin", DbType.DateTime, fechaFin);
            Database.AddOutParameter(comandoKardex, "pCantidadInicial", DbType.Decimal, 0);
            Database.AddOutParameter(comandoKardex, "pCostoInicial", DbType.Decimal, 0);

            using (var dr = Database.ExecuteReader(comandoKardex))
            {
                while (dr.Read())
                {
                    int? idDocAlteno = null;
                    if (!dr.IsDBNull(dr.GetOrdinal("IdTipoDocumento")))
                        idDocAlteno = dr.GetInt32(dr.GetOrdinal("IdTipoDocumento"));

                    listaKardex.Add(new Kardex
                    {
                        CantidadIngreso = dr.GetDecimal(dr.GetOrdinal("CantidadIngreso")),
                        CantidadSaldo = dr.GetDecimal(dr.GetOrdinal("CantidadSaldo")),
                        CantidadSalida = dr.GetDecimal(dr.GetOrdinal("CantidadSalida")),
                        CodigoProducto = dr.GetString(dr.GetOrdinal("CodigoProducto")),
                        CostoTotalIngreso = dr.GetDecimal(dr.GetOrdinal("CostoTotalIngreso")),
                        CostoTotalSaldo = dr.GetDecimal(dr.GetOrdinal("CostoTotalSaldo")),
                        CostoTotalSalida = dr.GetDecimal(dr.GetOrdinal("CostoTotalSalida")),
                        CostoUnitarioIngreso = dr.GetDecimal(dr.GetOrdinal("CostoUnitarioIngreso")),
                        CostoUnitarioSaldo = dr.GetDecimal(dr.GetOrdinal("CostoUnitarioSaldo")),
                        CostoUnitarioSalida = dr.GetDecimal(dr.GetOrdinal("CostoUnitarioSalida")),
                        Estado = dr.GetInt32(dr.GetOrdinal("Estado")),
                        FechaEmision = dr.GetDateTime(dr.GetOrdinal("FechaEmision")),
                        FechaProceso = dr.GetDateTime(dr.GetOrdinal("FechaProceso")),
                        IdAlmacen = dr.GetInt32(dr.GetOrdinal("IdAlmacen")),
                        IdEmpleado = dr.GetInt32(dr.GetOrdinal("IdEmpleado")),
                        IdEmpresa = dr.GetInt32(dr.GetOrdinal("IdEmpresa")),
                        IdOperacion = dr.GetInt32(dr.GetOrdinal("IdOperacion")),
                        IdPresentacion = dr.GetInt32(dr.GetOrdinal("IdPresentacion")),
                        IdProducto = dr.GetInt32(dr.GetOrdinal("IdProducto")),
                        IdSucursal = dr.GetInt32(dr.GetOrdinal("IdSucursal")),
                        IdTipoDocumento = dr.GetInt32(dr.GetOrdinal("IdTipoDocumento")),
                        IdTipoDocumentoAlterno = idDocAlteno,
                        IdTransaccion = dr.GetInt32(dr.GetOrdinal("IdTransaccion")),
                        NumeroDocumento = dr.GetString(dr.GetOrdinal("NumeroDocumento")),
                        SerieDocumento = dr.GetString(dr.GetOrdinal("SerieDocumento")),
                        SignoStock = dr.GetInt32(dr.GetOrdinal("SignoStock")),
                        TipoDocumento = dr.GetString(dr.GetOrdinal("TipoDocumento")),
                        TipoOperacion = dr.GetString(dr.GetOrdinal("TipoOperacion"))
                    });
                }
            }

            cantidadInicial = Convert.ToDecimal(Database.GetParameterValue(comandoKardex, "pCantidadInicial"));
            costoTotalInicial = Convert.ToDecimal(Database.GetParameterValue(comandoKardex, "pCostoInicial"));

            return listaKardex;
        }
    }
}
