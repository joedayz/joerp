
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Business.Logic;
    using Core;
    using Helpers;
    using Microsoft.Reporting.WebForms;

    public class ReporteController : BaseController
    {
        #region Inventario Físico

        public ActionResult ReporteInventarioFisico()
        {
            return PartialView("ReporteInventarioFisico");
        }

        [HttpPost]
        public JsonResult InventarioFisicoCabecera( int linea, int sublinea, int categoria,
            string codigoProducto, int sucursal, int almacen, string fechaVencimiento, int idPresentacion)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var inventario = ReporteBL.Instancia.ListarInventarioFisico(1, linea, sublinea, categoria, codigoProducto, almacen, sucursal, fechaVencimiento, idPresentacion);
                var datos = from g in inventario
                            select new
                            {
                                IdPresentacion = g.IdPresentacion,
                                IdProducto = g.IdProducto,
                                Codigo = g.Codigo,
                                Producto = g.Producto,
                                Presentacion = g.Presentacion,
                                Equivalencia = g.Equivalencia,
                                Stock = g.StockFisico,
                            };
                jsonResponse.Success = true;
                jsonResponse.Data = datos;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InventarioFisicoDetalle(int linea, int sublinea, int categoria,
            string codigoProducto, int sucursal, int almacen, string fechaVencimiento, int idPresentacion)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var inventario = ReporteBL.Instancia.ListarInventarioFisico(2, linea, sublinea, categoria, codigoProducto,
                                almacen, sucursal, fechaVencimiento,idPresentacion);
                var datos = (from g in inventario
                            select new
                                    {
                                        IdPresentacion = g.IdPresentacion,
                                        Presentacion = g.Presentacion,
                                        Lote = g.Lote ?? string.Empty,
                                        Serie = g.Serie ?? string.Empty,
                                        Stock = g.StockFisico,
                                        FechaVencimiento = g.FechaVencimiento
                                    }).ToList();
                jsonResponse.Success = true;
                jsonResponse.Data = datos;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarInventarioFisicoCorte(string formato, int linea, int sublinea, int categoria,
           string codigoProducto, int almacen, string fechaCorte)
        {
            var datos = ReporteBL.Instancia.ListarInventarioFisicoCorte(linea, sublinea, categoria, codigoProducto,
                                almacen, fechaCorte);
            RenderReport("ReporteInventarioFisico", "DSInventarioFisico", datos, formato);
            return View("ReporteInventarioFisico");
        }


        public ActionResult ExportarInventarioFisico(string formato, int linea, int sublinea, int categoria,
            string codigoProducto, int sucursal, int almacen, string fechaVencimiento, int idPresentacion)
        {
            var datos = ReporteBL.Instancia.ListarInventarioFisico(3, linea, sublinea, categoria, codigoProducto,
                                almacen, sucursal, fechaVencimiento, idPresentacion);
            RenderReport("ReporteInventarioFisico", "DSInventarioFisico", datos, formato);
            return View("ReporteInventarioFisico");
        }

        public ActionResult ExportarMovimientoProductos(string formato, string codigoProducto, int sucursal, int almacen)
        {
            var datos = ReporteBL.Instancia.ListarMovimientosProductoLote(codigoProducto,sucursal,almacen);
            RenderReport("ReporteMovimientos", "DSMovimientos", datos, formato);
            return View("ReporteInventarioFisico");
        }

        public ActionResult ExportarInventarioCosto(string formato)
        {
            var datos = ReporteBL.Instancia.ListarInventarioFisico(5, 0, 0, 0, "", 0, 0, DateTime.Now.ToString(), 0);
            RenderReport("ReporteInventarioCosto", "DSInventarioCosto", datos, formato);
            return View("ReporteInventarioFisico");
        }

        public ActionResult ExportarCostoSalidas(string formato)
        {
            var datos = ReporteBL.Instancia.ListarInventarioFisico(4, 0, 0, 0, "", 0, 0, DateTime.Now.ToString(), 0);
            RenderReport("ReporteCostoSalidas", "DSCostoSalidas", datos, formato);
            return View("ReporteInventarioFisico");
        }

        public ActionResult ExportasVentaExcel(string id, string formato)
        {
            var idTransaccion = Convert.ToInt32(id);
            var datos = VentaBL.Instancia.ObtenerVentaExportar(idTransaccion);
            RenderReport("ReporteVenta", "DSVenta", datos, formato);
            return View("ReporteDetalleValorizacion");
        }

        public ActionResult ExportasCompraExcel(string id, string formato)
        {
            var idTransaccion = Convert.ToInt32(id);
            var datos = VentaBL.Instancia.ObtenerVentaExportar(idTransaccion);
            RenderReport("ReporteCompra", "DSVenta", datos, formato);
            return View("ReporteDetalleValorizacion");
        }

        #endregion Inventario Físico

        #region Kardex

        public ActionResult ReporteKardex()
        {
            return PartialView("ReporteKardex");
        }

        [HttpPost]
        public JsonResult ReporteKardex(int idEmpresa, int idSucursal,
                int idProducto, int idPresentacion, string fechaInicio, string fechaFin)
        {
            decimal cantidadTotalIngreso = 0;
            decimal costoTotalIngreso = 0;
            decimal cantidadTotalSalida = 0;
            decimal costoTotalSalida = 0;
            decimal cantidadInicial = 0;
            decimal costoTotalInicial = 0;
            decimal cantidadFinal = 0;
            decimal costoTotalFinal = 0;

            var jsonResponse = new JsonResponse();
            try
            {
                var fechaDesde = string.IsNullOrEmpty(fechaInicio) ? (DateTime?) null : DateTime.Parse(fechaInicio);
                var fechaHasta = string.IsNullOrEmpty(fechaFin) ? (DateTime?)null : DateTime.Parse(fechaFin);

                var inventario = KardexBL.Instancia.SelectKardexValorizado(idEmpresa, idSucursal,
                                                                           idProducto, idPresentacion, fechaDesde,
                                                                           fechaHasta, out cantidadTotalIngreso,
                                                                           out costoTotalIngreso,
                                                                           out cantidadTotalSalida, out costoTotalSalida,
                                                                           out cantidadInicial, out costoTotalInicial,
                                                                           out cantidadFinal, out costoTotalFinal);

                var datos = from g in inventario
                            select new
                                       {
                                           IdTransaccion = g.IdTransaccion,
                                           FechaProceso = g.FechaProceso.ToString("dd/MM/yyyy"),
                                           FechaEmision = g.FechaEmision.ToString("dd/MM/yyyy"),
                                           TipoDocumento = g.TipoDocumento,
                                           SerieDocumento = g.SerieDocumento,
                                           NumeroDocumento = g.NumeroDocumento,
                                           TipoOperacion = g.TipoOperacion,
                                           CantidadIngreso = g.CantidadIngreso,
                                           CostoUnitarioIngreso = g.CostoUnitarioIngreso,
                                           CostoTotalIngreso = g.CostoTotalIngreso,
                                           CantidadSalida = g.CantidadSalida,
                                           CostoUnitarioSalida = g.CostoUnitarioSalida,
                                           CostoTotalSalida = g.CostoTotalSalida,
                                           CantidadSaldo = g.CantidadSaldo,
                                           CostoUnitarioSaldo = g.CostoUnitarioSaldo,
                                           CostoTotalSaldo = g.CostoTotalSaldo,
                                           Estado = g.Estado
                                       };

                jsonResponse.Success = true;
                jsonResponse.Data = datos;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarKardex(int idEmpresa, int idSucursal,
                int idProducto, int idPresentacion, DateTime fechaInicio, DateTime fechaFin, out decimal cantidadTotalIngreso,
                out decimal costoTotalIngreso, out decimal cantidadTotalSalida, out decimal costoTotalSalida,
                out decimal cantidadInicial, out decimal costoTotalInicial, out decimal cantidadFinal, out decimal costoTotalFinal, string formato)
        {
           
            var datos = KardexBL.Instancia.SelectKardexValorizado(idEmpresa, idSucursal,
                idProducto, idPresentacion, fechaInicio, fechaFin, out cantidadTotalIngreso,
                out costoTotalIngreso, out cantidadTotalSalida, out costoTotalSalida,
                out cantidadInicial, out costoTotalInicial, out cantidadFinal, out costoTotalFinal);
            RenderReport("ReporteKardex", "DSKardex", datos, formato);
            return View("ReporteKardex");
        }

        #endregion Kardex

        #region Valorizaciones

        public ActionResult Valorizaciones()
        {
            return PartialView("Valorizaciones");
        }

        [HttpPost]
        public JsonResult ReporteValorizacion(string serie, string numeroDoc)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var valorizaciones = ReporteBL.Instancia.ListarValorizaciones(serie,numeroDoc,IdEmpresa,IdSucursal);

                var datos = from g in valorizaciones
                            select new
                            {
                                IdTransaccion = g.IdTransaccion,
                                FechaProceso = g.FechaCreacion.ToString("dd/MM/yyyy"),
                                NombreProducto = g.NombreProducto,
                                NombrePresentacion = g.NombrePresentacion,
                                Cantidad = g.Cantidad,
                                Costo = g.Costo,
                            };

                jsonResponse.Success = true;
                jsonResponse.Data = datos;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarValorizacion(string serie, string numeroDoc, string formato)
        {
            var datos = ReporteBL.Instancia.ListarValorizaciones(serie, numeroDoc, IdEmpresa, IdSucursal);
            RenderReport("Valorizaciones", "DSValorizaciones", datos, formato);
            return View("Valorizaciones");
        }

        #endregion Valorizaciones

        #region ProductoProveedor

        public ActionResult ProductoProveedor()
        {
            return PartialView("ReporteProductoProveedor");
        }

        [HttpPost]
        public JsonResult ReporteProductoProveedor(int linea, int sublinea, int categoria, int idProveedor, int idProducto)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var productos = ReporteBL.Instancia.ListarProductoProveedor(linea,sublinea,categoria,idProveedor,idProducto);

                var datos = from g in productos
                            select new
                            {
                                Codigo = g.Codigo,
                                CodigoAlterno = g.CodigoAlterno,
                                Producto = g.Producto,
                                Linea = g.Linea,
                                SubLinea = g.SubLinea,
                                Categoria = g.Categoria,
                                Proveedor = g.Proveedor,
                                Precio = g.Precio
                            };

                jsonResponse.Success = true;
                jsonResponse.Data = datos;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarProductoProveedor(int linea, int sublinea, int categoria, int idProveedor, int idProducto, string formato)
        {
            var datos = ReporteBL.Instancia.ListarProductoProveedor(linea, sublinea, categoria, idProveedor, idProducto);
            RenderReport("ProductoProveedor", "DSProductoProveedor", datos, formato);
            return View("Valorizaciones");
        }

        #endregion ProductoProveedor

        #region Detalle de Valorizaciones

        public ActionResult DetalleValorizacion()
        {
            return PartialView("ReporteDetalleValorizacion");
        }

        [HttpPost]
        public JsonResult ReporteDetalleValorizacion(int idTransaccion)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var productos = ReporteBL.Instancia.ListarDetalleValorizacion(idTransaccion);

                var datos = from g in productos
                            select new
                            {
                                IdTransaccion  = g.IdTransaccion,
                                FechaDocumento = g.FechaFormat,
                                Documento = g.Documento, 
                                SerieDocumento = g.SerieDocumento,
                                NumeroDocumento = g.NumeroDocumento,
                                Comentarios = g.Comentarios,
                                MonedaNombre = g.MonedaNombre,
                                TipoCambio = g.TipoCambio,
                                Monto = g.Monto
                            };

                jsonResponse.Success = true;
                jsonResponse.Data = datos;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarReporteDetalleValorizacion(string formato)
        {
            var datos = ReporteBL.Instancia.ListarDetalleValorizacionExcel(IdEmpresa, IdSucursal);
            
            var sb = new StringBuilder();
            sb.Append("<table cellpadding=5 cellspacing=0>");
            sb.Append("<tr>");

            sb.Append("<td style='width:80px;'><b><font face=Arial size=2>Fecha</font></b></td>");
            sb.Append("<td style='width:200px'><b><font face=Arial size=2>Documento</font></b></td>");
            sb.Append("<td style='width:50px'><b><font face=Arial size=2>SerieDocumento</font></b></td>");
            sb.Append("<td style='width:80px'><b><font face=Arial size=2>NumeroDocumento</font></b></td>");
            sb.Append("<td style='width:350px'><b><font face=Arial size=2>Comentarios</font></b></td>");
            sb.Append("<td style='width:100px'><b><font face=Arial size=2>MonedaNombre</font></b></td>");
            sb.Append("<td style='width:80px'><b><font face=Arial size=2>TipoCambio</font></b></td>");
            sb.Append("<td style='width:80px'><b><font face=Arial size=2>Monto</font></b></td>");
            
            sb.Append("</tr>");

            foreach (var dt in datos)
            {
                if (String.Compare(dt.FechaFormat, "COMPRA", StringComparison.Ordinal) == 0)
                {
                    sb.Append("<tr><td colspan=8></td></tr><tr><td colspan=8 bgcolor=#DADADA><b><font face=Arial size=2>COMPRA</font></b></td></tr>");
                }
                else
                {
                    sb.Append("<tr>");

                    sb.Append("<td style='width:80px'><font face=Arial size=2>" + dt.FechaFormat + "</font></td>");
                    sb.Append("<td style='width:200px'><font face=Arial size=2>" + dt.Documento + "</font></td>");
                    sb.Append("<td style='width:50px'><font face=Arial size=2>" + dt.SerieDocumento + "</font></td>");
                    sb.Append("<td style='width:80px'><font face=Arial size=2>" + dt.NumeroDocumento + "</font></td>");
                    sb.Append("<td style='width:350px'><font face=Arial size=2>" + dt.Comentarios + "</font></td>");
                    sb.Append("<td style='width:100px'><font face=Arial size=2>" + dt.MonedaNombre + "</font></td>");
                    sb.Append("<td style='width:80px'><font face=Arial size=2>" + dt.TipoCambio + "</font></td>");
                    sb.Append("<td style='width:80px'><font face=Arial size=2>" + dt.Monto + "</font></td>");

                    sb.Append("</tr>");
                }
            }
            sb.Append("</table>");

            Response.AddHeader("Content-Disposition", "Reporte.xls");
            Response.ContentType = "application/vnd.ms-excel";
            var buffer = Encoding.GetEncoding(1252).GetBytes(sb.ToString());
            return File(buffer, "application/vnd.ms-excel");
        }

        #endregion Detalle de Valorizaciones

        #region RenderReport

        public void RenderReport(string report, string ds, object data, string formato)
        {
            var reportPath = Server.MapPath("~/Reportes/" + report + ".rdlc");
            var localReport = new LocalReport { ReportPath = reportPath };
            var reportDataSource = new ReportDataSource(ds, data);

            localReport.DataSources.Add(reportDataSource);

            var reportType = string.Empty;
            var deviceInfo = string.Empty;

            switch (formato)
            {
                case "PDF":
                    reportType = "PDF";
                    deviceInfo = string.Format("<DeviceInfo><OutputFormat>{0}</OutputFormat><PageWidth>8.5in</PageWidth><PageHeight>11in</PageHeight><MarginTop>0.5in</MarginTop><MarginLeft>0.5in</MarginLeft><MarginRight>0.5in</MarginRight><MarginBottom>0.5in</MarginBottom></DeviceInfo>", reportType);
                    break;
                case "EXCEL":
                    reportType = "Excel";
                    break;
                default:
                    return;
            }

            string mimeType, encoding, fileNameExtension;
            Warning[] warnings;
            string[] streams;

            byte[] renderedBytes;

            try
            {
                renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + report + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        #endregion RenderReport
    }
}
