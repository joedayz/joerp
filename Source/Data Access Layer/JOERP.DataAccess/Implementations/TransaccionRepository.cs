
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Business.Entity.DTO;
    using Interfaces;

    public class TransaccionRepository : Repository<Transaccion>, ITransaccionRepository
    {
        public List<TransaccionMovimientoDTO> SelectTransaccionByParameter(DateTime? fechaInicio, DateTime? fechaFin, int idPersona,
            int idOperacion, int idSucursal, int idAlmacen, int idProducto, int idPresentacion, string serie, string numero)
        {
            var listaTransaccion = new List<TransaccionMovimientoDTO>();

            var comandoTransaccion = Database.GetStoredProcCommand("SelectTransaccionByParameter");
            Database.AddInParameter(comandoTransaccion, "pFechaInicial", DbType.DateTime, fechaInicio);
            Database.AddInParameter(comandoTransaccion, "pFechaFinal", DbType.DateTime, fechaFin);
            Database.AddInParameter(comandoTransaccion, "pIdPersona", DbType.Int32, idPersona);
            Database.AddInParameter(comandoTransaccion, "pIdOperacion", DbType.Int32, idOperacion);
            Database.AddInParameter(comandoTransaccion, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(comandoTransaccion, "pIdAlmacen", DbType.Int32, idAlmacen);
            Database.AddInParameter(comandoTransaccion, "pIdProducto", DbType.Int32, idProducto);
            Database.AddInParameter(comandoTransaccion, "pIdPresentacion", DbType.Int32, idPresentacion);
            Database.AddInParameter(comandoTransaccion, "pSerie", DbType.String, serie);
            Database.AddInParameter(comandoTransaccion, "pNumero", DbType.String, numero);

            using (var dr = Database.ExecuteReader(comandoTransaccion))
            {
                while (dr.Read())
                {
                    var transaccionNueva = new TransaccionMovimientoDTO();
                    transaccionNueva.IdTransaccion = dr.GetInt32(dr.GetOrdinal("IdTransaccion"));
                    transaccionNueva.AlmacenDestino = dr.GetString(dr.GetOrdinal("AlmacenDestino"));
                    transaccionNueva.AlmacenOrigen = dr.GetString(dr.GetOrdinal("AlmacenOrigen"));
                    transaccionNueva.Direccion = dr.GetString(dr.GetOrdinal("Direccion"));
                    transaccionNueva.Documento = dr.GetString(dr.GetOrdinal("Documento"));
                    transaccionNueva.DocumentoRelacionado = dr.GetString(dr.GetOrdinal("DocumentoRelacionado"));
                    transaccionNueva.Estado = dr.GetInt32(dr.GetOrdinal("Estado"));
                    transaccionNueva.FechaRegistro = dr.GetDateTime(dr.GetOrdinal("FechaRegistro"));
                    transaccionNueva.MontoTipoCambio = dr.GetDecimal(dr.GetOrdinal("MontoTipoCambio"));
                    transaccionNueva.NombreEmpleado = dr.GetString(dr.GetOrdinal("NombreEmpleado"));
                    transaccionNueva.NombreEmpresa = dr.GetString(dr.GetOrdinal("NombreEmpresa"));
                    transaccionNueva.NombreEmpresaTransporte = dr.GetString(dr.GetOrdinal("NombreEmpresaTransporte"));
                    transaccionNueva.NombreMoneda = dr.GetString(dr.GetOrdinal("NombreMoneda"));
                    transaccionNueva.NombreOperacion = dr.GetString(dr.GetOrdinal("NombreOperacion"));
                    transaccionNueva.NombreSucursal = dr.GetString(dr.GetOrdinal("NombreSucursal"));
                    transaccionNueva.NombreSucursal1 = dr.GetString(dr.GetOrdinal("NombreSucursal1"));
                    transaccionNueva.PersonaConductor = dr.GetString(dr.GetOrdinal("PersonaConductor"));
                    transaccionNueva.Proveedor = dr.GetString(dr.GetOrdinal("Proveedor"));

                    listaTransaccion.Add(transaccionNueva);
                }
            }

            return listaTransaccion;
        }

        public int InsertTransaccion(Transaccion transaccion, DbTransaction transaction = null)
        {
            Add("usp_Insert_Transaccion", transaccion, transaction);
            return transaccion.IdTransaccion;
        }

        public void UpdateTransaccion(Transaccion transaccion, DbTransaction transaction = null)
        {
            Update("usp_Update_Transaccion", transaccion, transaction);
        }

        public void DeleteTransaccion(int idTransaccion, int estado, int usuario, DbTransaction transaction = null)
        {
            var comandoDeleteTransaccion = Database.GetStoredProcCommand("DeleteTransaccion");
            Database.AddInParameter(comandoDeleteTransaccion, "pIdTransaccion", DbType.Int32, idTransaccion);
            Database.AddInParameter(comandoDeleteTransaccion, "pEstado", DbType.Int32, estado);
            Database.AddInParameter(comandoDeleteTransaccion, "pUsuarioModificacion", DbType.Int32, usuario);
            Database.ExecuteNonQuery(comandoDeleteTransaccion, transaction);
        }

        public void UpdateReferenciaTransaccion(Transaccion transaccion, DbTransaction transaction = null)
        {
            Update("Update_ReferenciaTransaccion", transaccion, transaction);
        }
    }
}
