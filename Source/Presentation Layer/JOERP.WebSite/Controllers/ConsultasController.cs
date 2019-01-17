
namespace JOERP.WebSite.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

    public class ConsultasController : BaseController
    {
        public JsonResult CargarCompras()
        {
            var compras = CompraBL.Instancia.GetAllByEmpresaSucursal(IdEmpresa, IdSucursal);
            var listItems = Utils.ConvertToListItem(compras, "IdTransaccion", "NumeroDocumento");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarAlmacenes(int idSucursal)
        {
            if (idSucursal == 0)
            {
                idSucursal = IdSucursal;
            }
            var almacenes = AlmacenBL.Instancia.GetByIdSucursal(idSucursal);
            var listItems = Utils.ConvertToListItem(almacenes, "IdAlmacen", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarSerieDocumento(int tipoDocumento)
        {
            var series = SerieDocumentoBL.Instancia.GetByTipoDocumento(IdSucursal, tipoDocumento);
            var listItems = Utils.ConvertToListItem(series, "IdSerieDocumento", "Serie", false);
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarNumeroDocumento(int idSerieDocumento)
        {
            if (idSerieDocumento == 0) return null;
            var serieDocumento = SerieDocumentoBL.Instancia.Single(idSerieDocumento);
            var numeroDocumento = serieDocumento.NumeroActual.PadLeft(8, '0');
            return Json(numeroDocumento, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult BuscarProveedorRs(string funcion, string term)
        {
            var resultado = new List<object>();
            var proveedores = PersonaBL.Instancia.GetByFuncionFiltro(Convert.ToInt32(funcion), term);

            proveedores = proveedores.OrderBy(p => p.FullName).ToList();
            proveedores.ForEach(p => resultado.Add(new
            {
                id = p.IdPersona,
                label = p.FullName,
                value = p.FullName,
                ruc = p.Documento,
                razonSocial = p.FullName
            }));

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult BuscarProveedorRuc(string funcion, string term)
        {
            var resultado = new List<object>();
            var proveedores = PersonaBL.Instancia.GetByFuncionFiltro(Convert.ToInt32(funcion), term);

            proveedores = proveedores.OrderBy(p => p.FullName).ToList();
            proveedores.ForEach(p => resultado.Add(new
            {
                id = p.IdPersona,
                label = p.Documento,
                value = p.Documento,
                ruc = p.Documento,
                razonSocial = p.FullName
            }));

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarProductoAutocompletarByCodigo(string term)
        {
            var resultado = new List<object>();
            var productos = ProductoBL.Instancia.GetByCodigo(IdEmpresa, term);

            productos = productos.OrderBy(p => p.Nombre).ToList();
            productos.ForEach(p => resultado.Add(new
            {
                id = p.IdProducto,
                value = p.Codigo,
                nombre = p.Nombre,
                tipo = p.TipoClasificacion,
                codigo = p.Codigo,
                alterno = p.CodigoAlterno
            }));

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarProductoAutocompletar(string term)
        {
            var resultado = new List<object>();
            var productos = ProductoBL.Instancia.GetByNombre(term);

            productos = productos.OrderBy(p => p.Nombre).ToList();
            productos.ForEach(p => resultado.Add(new
            {
                id = p.IdProducto,
                value = p.Nombre,
                codigo = p.Codigo,
                tipo = p.TipoClasificacion,
                nombre = p.Nombre,
                alterno = p.CodigoAlterno
            }));

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarProductoAutocompletarByCodigoAlterno(string term)
        {
            var resultado = new List<object>();
            var productos = ProductoBL.Instancia.GetByCodigoAlterno(IdEmpresa, term);

            productos = productos.OrderBy(p => p.Nombre).ToList();
            productos.ForEach(p => resultado.Add(new
            {
                id = p.IdProducto,
                value = p.CodigoAlterno,
                nombre = p.Nombre,
                tipo = p.TipoClasificacion,
                codigo = p.Codigo,
                alterno = p.CodigoAlterno
            }));

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarPresentaciones(int idProducto)
        {
            var presentaciones = PresentacionBL.Instancia.GetByIdProducto(idProducto);
            return Json(presentaciones, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarLotesProducto(int idPresentacion, int idAlmacen)
        {
            var lotes = ProductoBL.Instancia.GetLotesProductoByAlmacen(idPresentacion, idAlmacen);
            return Json(lotes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarLotesProductoFiltro(int idPresentacion, int idAlmacen,string term)
        {
            var resultado = new List<Object>();
            var lotes = ProductoBL.Instancia.GetLotesProductoByAlmacen(idPresentacion, idAlmacen);
            lotes = lotes.Where(p => p.Lote.StartsWith(term)).ToList();
            lotes = lotes.OrderBy(p => p.Lote).ToList();
            lotes.ForEach(p => resultado.Add(new
            {
                id = p.Lote,
                value = p.Lote,
                fv = p.FechaVencimiento,
            }));
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarNumerosDocumentos(int idSerieDocumento, int idOperacion)
        {
            var numerosDocumentos = CompraBL.Instancia.GetNumerosDocumentosByIdSerieDocumento(idOperacion, idSerieDocumento);
            var listItems = Utils.ConvertToListItem(numerosDocumentos, "Id", "Valor", false);
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarTipoCambio(int idMoneda, string fecha)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                if (MonedaLocal != null)
                {
                    if (idMoneda == MonedaLocal.IdMoneda)
                    {
                        jsonResponse.Data = new TipoCambio { ValorVenta = 1, ValorCompra = 1};
                    }
                    else
                    {
                        var fechaCambio = Convert.ToDateTime(fecha);
                        var tipoCambio = TipoCambioBL.Instancia.SingleByMonedaFecha(idMoneda, fechaCambio);

                        jsonResponse.Data = tipoCambio;
                    }
                }
                else
                {
                    var fechaCambio = Convert.ToDateTime(fecha);
                    var tipoCambio = TipoCambioBL.Instancia.SingleByMonedaFecha(idMoneda, fechaCambio);

                    jsonResponse.Data = tipoCambio;
                }

                jsonResponse.Success = true;
            }
            catch (Exception ex)
            {
                jsonResponse.Data = ex.Message;
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
         }

    }
}
