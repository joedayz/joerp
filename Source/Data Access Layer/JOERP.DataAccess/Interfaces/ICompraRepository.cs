
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers;

    public interface ICompraRepository : IRepository<Compra>
    {
        bool Insertar(Transaccion transaccion);

        bool Actualizar(Transaccion transaccion);

        bool Eliminar(Transaccion transaccion);

        List<SerieDocumento> GetSeriesOrdenCompraAprobados(int idEmpresa, int idSucursal, int tipoDocumento, int estado);

        List<Comun> GetNumerosOrdenCompraAprobados(int idEmpresa, int idSucursal, int idSerie, int estado);

        IList<Comun> GetNumerosDocumentosByIdSerieDocumento(int idOperacion, int idSerieDocumento);

        Transaccion GetDatosDocumento(int idTransaccion);

        Compra Single(int idTransaccion, DbTransaction transaction = null);

        IList<Transaccion> GetAllByEmpresaSucursal(int idEmpresa, int idSucursal);

        IList<Compra> BuscarCompras(int idOperacion, int idProveedor, string desde, string hasta, string documento);
    }
}
