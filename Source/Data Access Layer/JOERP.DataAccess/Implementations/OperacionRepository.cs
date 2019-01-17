
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Data;
    using Business.Entity;
    using Interfaces;

    public class OperacionRepository : Repository<Operacion>, IOperacionRepository
    {
        public int MaxId()
        {
            return (int) GetScalar("usp_MaxId_Operacion");
        }

        public bool ExisteCodigo(string codigo, int id)
        {
            return (int) GetScalar("usp_ExisteCodigo_Operacion", codigo, id) > 0;
        }

        public Operacion Add(Operacion entity)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        var operacion = Add("usp_Insert_Operacion", entity);

                        var documentos = entity.OperacionDocumentos;
                        var comandoInsertarDocumento = Database.GetStoredProcCommand("usp_Insert_OperacionDocumento");
                        Database.AddInParameter(comandoInsertarDocumento, "IdOperacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "TipoDocumento", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "Orden", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "Posicion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "Estado", DbType.Int32);


                        foreach (var documento in documentos)
                        {
                            Database.SetParameterValue(comandoInsertarDocumento, "IdOperacion", operacion.IdOperacion);
                            Database.SetParameterValue(comandoInsertarDocumento, "TipoDocumento",documento.TipoDocumento);
                            Database.SetParameterValue(comandoInsertarDocumento, "Orden", documento.Orden);
                            Database.SetParameterValue(comandoInsertarDocumento, "Posicion", documento.Posicion);
                            Database.SetParameterValue(comandoInsertarDocumento, "Estado", documento.Estado);

                            Database.ExecuteNonQuery(comandoInsertarDocumento, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            return entity;
        }

        public Operacion Update(Operacion entity)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        var operacion = Add("usp_Update_Operacion", entity);

                        var comandoEliminarDocumento = Database.GetStoredProcCommand("usp_Delete_OperacionDocumento");
                        Database.AddInParameter(comandoEliminarDocumento,"IdOperacion",DbType.Int32);
                        Database.SetParameterValue(comandoEliminarDocumento,"IdOperacion",entity.IdOperacion);
                        Database.ExecuteNonQuery(comandoEliminarDocumento, transaction);

                        var documentos = entity.OperacionDocumentos;
                        var comandoInsertarDocumento = Database.GetStoredProcCommand("usp_Insert_OperacionDocumento");
                        Database.AddInParameter(comandoInsertarDocumento, "IdOperacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "TipoDocumento", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "Orden", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "Posicion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarDocumento, "Estado", DbType.Int32);


                        foreach (var documento in documentos)
                        {
                            Database.SetParameterValue(comandoInsertarDocumento, "IdOperacion", operacion.IdOperacion);
                            Database.SetParameterValue(comandoInsertarDocumento, "TipoDocumento",documento.TipoDocumento);
                            Database.SetParameterValue(comandoInsertarDocumento, "Orden", documento.Orden);
                            Database.SetParameterValue(comandoInsertarDocumento, "Posicion", documento.Posicion);
                            Database.SetParameterValue(comandoInsertarDocumento, "Estado", documento.Estado);

                            Database.ExecuteNonQuery(comandoInsertarDocumento, transaction);
                        }
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            return entity;
        }
    }
}
