
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Data.Common;
    using Business.Entity;
    using Helpers;
    using Helpers.Enums;
    using Interfaces;

    public class TipoCambioRepository : Repository<TipoCambio>, ITipoCambioRepository
    {
        private readonly IMonedaRepository monedaRepository = new MonedaRepository();

        public TipoCambio GetTipoCambio(int idTipoCambio, DbTransaction transaction = null)
        {
            TipoCambio tipoCambio = null;

            using (var comando = Database.GetSqlStringCommand("SELECT * FROM tipocambio WHERE IdTipoCambio = " + idTipoCambio))
            {
                var dr = transaction == null ? Database.ExecuteReader(comando) : Database.ExecuteReader(comando, transaction);

                if (dr.Read())
                {
                    tipoCambio = new TipoCambio
                    {
                        IdTipoCambio = dr.GetInt32(dr.GetOrdinal("IdTipoCambio")),
                        IdMoneda = dr.GetInt32(dr.GetOrdinal("IdMoneda")),
                        Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                        TipoCalculo = dr.GetInt32(dr.GetOrdinal("TipoCalculo")),
                        ValorCompra = dr.GetDecimal(dr.GetOrdinal("ValorCompra")).Redondear(),
                        ValorVenta = dr.GetDecimal(dr.GetOrdinal("ValorVenta")).Redondear(),
                        IdEmpresa = dr.GetInt32(dr.GetOrdinal("IdEmpresa"))
                    };
                }
                if (!dr.IsClosed)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            return tipoCambio;
        }

        public decimal GetMontoEnMonedaLocal(int idMoneda, decimal monto, DateTime fechaCambio, int idEmpresa, bool valorCompra)
        {
            try
            {
                var monedaLocal = monedaRepository.GetModenaLocal(idEmpresa);

                if (monedaLocal == null)
                {
                    throw new Exception("No existe una moneda local asignada para la Empresa en sesión.");
                }

                if (monedaLocal.IdMoneda != idMoneda)
                {
                    fechaCambio = new DateTime(fechaCambio.Year, fechaCambio.Month, fechaCambio.Day, 0, 0, 0);

                    var tipoCambio = Single("usp_Select_TipoCambio_ByEmpresaMonedaAndFecha", idEmpresa, idMoneda, fechaCambio);

                    if (tipoCambio == null)
                    {
                        throw new Exception(string.Format("No existe el tipo de cambio la fecha: {0} para la Moneda", fechaCambio.ToString("dd/MM/yyyy")));
                    }

                    monto = monto.Redondear();
                    tipoCambio.ValorCompra = tipoCambio.ValorCompra.Redondear();
                    tipoCambio.ValorVenta = tipoCambio.ValorVenta.Redondear();

                    switch (tipoCambio.TipoCalculo)
                    {
                        case (int) TipoCalculoMoneda.Multiplicar:
                            if (valorCompra)
                            {
                                monto = monto*tipoCambio.ValorCompra;
                            }
                            else
                            {
                                monto = monto*tipoCambio.ValorVenta;
                            }
                            break;
                        case (int) TipoCalculoMoneda.Dividir:
                            if (valorCompra)
                            {
                                monto = monto/tipoCambio.ValorCompra;
                            }
                            else
                            {
                                monto = monto/tipoCambio.ValorVenta;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la conversión a la moneda local " + ex.Message);
            }
            return monto.Redondear();
        }

        public TipoCambio SingleByMonedaFecha(int idMoneda, DateTime fecha)
        {
            var tipoCambio = Single("usp_Select_TipoCambio_SingleByMonedaFecha", idMoneda, fecha);
            
            tipoCambio.ValorCompra = tipoCambio.ValorCompra.Redondear();
            tipoCambio.ValorVenta = tipoCambio.ValorVenta.Redondear();

            return tipoCambio;
        }
    }
}
