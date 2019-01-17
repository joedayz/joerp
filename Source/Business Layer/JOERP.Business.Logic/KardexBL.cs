
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class KardexBL : Singleton<KardexBL>
    {
        private readonly IKardexRepository repository = new KardexRepository();

        public IList<Kardex> SelectKardexValorizado(int idEmpresa, int idSucursal, int idProducto, int idPresentacion,
                DateTime? fechaInicio, DateTime? fechaFin, out decimal cantidadTotalIngreso,
                out decimal costoTotalIngreso, out decimal cantidadTotalSalida, out decimal costoTotalSalida,
                out decimal cantidadInicial, out decimal costoTotalInicial, out decimal cantidadFinal, out decimal costoTotalFinal)
        {
            try
            {
                cantidadTotalIngreso = 0;
                costoTotalIngreso = 0;
                cantidadTotalSalida = 0;
                costoTotalSalida = 0;
                cantidadFinal = 0;
                costoTotalFinal = 0;

                var listaKardex = repository.SelectKardexValorizado(idEmpresa, idSucursal, idProducto, idPresentacion, fechaInicio, fechaFin, out cantidadInicial, out costoTotalInicial);

                foreach (var kardex in listaKardex)
                {
                    cantidadTotalIngreso += kardex.CantidadIngreso;
                    cantidadTotalSalida += kardex.CantidadSalida;
                    costoTotalIngreso += kardex.CostoTotalIngreso;
                    costoTotalSalida += kardex.CostoTotalSalida;
                }

                var ultimo = listaKardex.LastOrDefault();
                if (ultimo != null)
                {
                    cantidadFinal = ultimo.CantidadSaldo;
                    costoTotalFinal = ultimo.CostoTotalSaldo;
                }

                return listaKardex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
