
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Business.Entity;
    using Interfaces;

    public class PersonaRepository : Repository<Persona>, IPersonaRepository
    {
        public void DeleteFull(int idPersona)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        Delete("DeleteFuncion", idPersona);
                        Delete("DeleteDireccion", idPersona);
                        Delete("usp_Delete_Usuario", idPersona);
                        Delete("usp_Delete_Persona", idPersona);
                  
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                            throw new Exception(ex.InnerException.Message);
                        throw new Exception(ex.Message);
                    }
                }

            }
        }

        public Persona Update(Persona entity)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Persona

                        var comandoInsertarPersona = Database.GetStoredProcCommand("UpdatePersona");
                        Database.AddInParameter(comandoInsertarPersona, "IdPersona", DbType.Int32,entity.IdPersona);
                        Database.AddInParameter(comandoInsertarPersona, "Nombres", DbType.String, entity.Nombres);
                        Database.AddInParameter(comandoInsertarPersona, "TipoPersona", DbType.Int32, entity.TipoPersona);
                        Database.AddInParameter(comandoInsertarPersona, "Documento", DbType.String, entity.Documento);
                        Database.AddInParameter(comandoInsertarPersona, "TipoDocumento", DbType.Int32, entity.TipoDocumento);
                        Database.AddInParameter(comandoInsertarPersona, "EsPercepcion", DbType.Boolean, entity.EsPercepcion);
                        Database.AddInParameter(comandoInsertarPersona, "Sexo", DbType.Boolean, entity.Sexo);
                        Database.AddInParameter(comandoInsertarPersona, "Telefono", DbType.String, entity.Telefono);
                        Database.AddInParameter(comandoInsertarPersona, "Celular", DbType.String, entity.Celular);
                        Database.AddInParameter(comandoInsertarPersona, "IdEmpresa", DbType.Int32, entity.IdEmpresa);
                        Database.AddInParameter(comandoInsertarPersona, "ApellidoPaterno", DbType.String, entity.ApellidoPaterno);
                        Database.AddInParameter(comandoInsertarPersona, "ApellidoMaterno", DbType.String, entity.ApellidoMaterno);
                        Database.AddInParameter(comandoInsertarPersona, "RazonSocial", DbType.String, entity.RazonSocial);
                        Database.AddInParameter(comandoInsertarPersona, "FullName", DbType.String, entity.FullName);
                        Database.AddInParameter(comandoInsertarPersona, "Email", DbType.String, entity.Email);
                        Database.AddInParameter(comandoInsertarPersona, "Fax", DbType.String, entity.Fax);
                        Database.AddInParameter(comandoInsertarPersona, "PaginaWeb", DbType.String, entity.PaginaWeb);
                        Database.AddInParameter(comandoInsertarPersona, "CodigoPostal", DbType.String, entity.CodigoPostal);
                        Database.AddInParameter(comandoInsertarPersona, "ActividadEconomica", DbType.Int32, entity.ActividadEconomica);
                        Database.AddInParameter(comandoInsertarPersona, "NumeroLicencia", DbType.String, entity.NumeroLicencia);
                        Database.AddInParameter(comandoInsertarPersona, "Codigo", DbType.String, entity.Codigo);
                        Database.AddInParameter(comandoInsertarPersona, "FechaCreacion", DbType.DateTime, entity.FechaCreacion);
                        Database.AddInParameter(comandoInsertarPersona, "FechaModificacion", DbType.DateTime, entity.FechaModificacion);
                        Database.AddInParameter(comandoInsertarPersona, "UsuarioCreacion", DbType.Int32, entity.UsuarioCreacion);
                        Database.AddInParameter(comandoInsertarPersona, "UsuarioModificacion", DbType.Int32, entity.UsuarioModificacion);
                        Database.AddInParameter(comandoInsertarPersona, "Estado", DbType.Int32, entity.Estado);
                        Database.AddInParameter(comandoInsertarPersona, "EsExtranjero", DbType.Boolean, entity.EsExtranjero);
                        Database.AddInParameter(comandoInsertarPersona, "Pais", DbType.Int32, entity.Pais);
                        Database.AddInParameter(comandoInsertarPersona, "Direccion", DbType.String, entity.Direccion);
                        Database.AddInParameter(comandoInsertarPersona, "EstadoCivil", DbType.Int32, entity.EstadoCivil);
                        Database.AddInParameter(comandoInsertarPersona, "FechaNacimiento", DbType.DateTime, entity.FechaNacimiento);

                        Database.ExecuteNonQuery(comandoInsertarPersona, transaction);
                       // entity.IdPersona = Convert.ToInt32(Database.GetParameterValue(comandoInsertarPersona, "IdPersona"));

                        #endregion

                        #region Funcion
                        if (entity.PersonaFunciones != null)
                        {
                            var comandoEliminarPersonaFuncion = Database.GetStoredProcCommand("DeleteFuncion");
                            Database.AddInParameter(comandoEliminarPersonaFuncion, "IdPersona", DbType.Int32,
                                                    entity.IdPersona);
                           
                            Database.ExecuteNonQuery(comandoEliminarPersonaFuncion, transaction);

                            var comandoInsertarPersonaFuncion = Database.GetStoredProcCommand("InsertFuncion");
                            Database.AddInParameter(comandoInsertarPersonaFuncion, "IdFuncion", DbType.Int32);
                            Database.AddInParameter(comandoInsertarPersonaFuncion, "IdPersona", DbType.Int32);

                            foreach (var personaFuncion in entity.PersonaFunciones)
                            {
                                Database.SetParameterValue(comandoInsertarPersonaFuncion, "IdFuncion",personaFuncion.IdFuncion);
                                Database.SetParameterValue(comandoInsertarPersonaFuncion, "IdPersona",entity.IdPersona);

                                Database.ExecuteNonQuery(comandoInsertarPersonaFuncion, transaction);
                            }
                        }

                        #endregion

                        #region Producto
                        if (entity.ProductosProveedor != null)
                        {
                            var comandoInsertarPersonaProducto = Database.GetStoredProcCommand("InsertProductoProveedor");
                            Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPersona", DbType.Int32);
                            Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPresentacion", DbType.Int32);
                            Database.AddInParameter(comandoInsertarPersonaProducto, "pPrecio", DbType.Decimal);

                            foreach (ProductoProveedor proveedorProducto in entity.ProductosProveedor)
                            {
                                Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPersona",
                                                           entity.IdPersona);
                                Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPresentacion",
                                                           proveedorProducto.IdPresentacion);
                                Database.SetParameterValue(comandoInsertarPersonaProducto, "pPrecio",
                                                           proveedorProducto.Precio);

                                Database.ExecuteNonQuery(comandoInsertarPersonaProducto, transaction);
                            }
                        }

                        #endregion

                        #region Direccion

                        Delete("usp_Delete_PersonaDireccion",entity.IdPersona);

                        var comandoPersonaDireccion = Database.GetStoredProcCommand("InsertDireccion");
                        Database.AddOutParameter(comandoPersonaDireccion, "IdDireccion", DbType.Int32, 11);
                        Database.AddInParameter(comandoPersonaDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioModificacion", DbType.Int32);

                        foreach (PersonaDireccion personaDireccion in entity.PersonaDirecciones)
                        {
                            Database.SetParameterValue(comandoPersonaDireccion, "IdPersona", entity.IdPersona);
                            Database.SetParameterValue(comandoPersonaDireccion, "IdUbigeo", personaDireccion.IdUbigeo);
                            Database.SetParameterValue(comandoPersonaDireccion, "Referencia", personaDireccion.Referencia);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoVia", personaDireccion.TipoVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreVia", personaDireccion.NombreVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "Numero", personaDireccion.Numero);
                            Database.SetParameterValue(comandoPersonaDireccion, "Interior", personaDireccion.Interior);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoDireccion", personaDireccion.TipoDireccion);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoZona", personaDireccion.TipoZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreZona", personaDireccion.NombreZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaCreacion", personaDireccion.FechaCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaModificacion", personaDireccion.FechaModificacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioCreacion", personaDireccion.UsuarioCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioModificacion", personaDireccion.UsuarioModificacion);

                            var resultado = Database.ExecuteNonQuery(comandoPersonaDireccion, transaction);
                        }

                        #endregion
                        
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                            throw new Exception(ex.InnerException.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }

            return entity;

        }

        public Persona AddT(Persona entity)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Persona

                        var comandoInsertarPersona = Database.GetStoredProcCommand("InsertPersona");
                        Database.AddOutParameter(comandoInsertarPersona, "IdPersona", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarPersona, "Nombres", DbType.String, entity.Nombres);
                        Database.AddInParameter(comandoInsertarPersona, "TipoPersona", DbType.Int32, entity.TipoPersona);
                        Database.AddInParameter(comandoInsertarPersona, "Documento", DbType.String, entity.Documento);
                        Database.AddInParameter(comandoInsertarPersona, "TipoDocumento", DbType.Int32, entity.TipoDocumento);
                        Database.AddInParameter(comandoInsertarPersona, "EsPercepcion", DbType.Boolean, entity.EsPercepcion);
                        Database.AddInParameter(comandoInsertarPersona, "Sexo", DbType.Boolean, entity.Sexo);
                        Database.AddInParameter(comandoInsertarPersona, "Telefono", DbType.String, entity.Telefono);
                        Database.AddInParameter(comandoInsertarPersona, "Celular", DbType.String, entity.Celular);
                        Database.AddInParameter(comandoInsertarPersona, "IdEmpresa", DbType.Int32, entity.IdEmpresa);
                        Database.AddInParameter(comandoInsertarPersona, "ApellidoPaterno", DbType.String, entity.ApellidoPaterno);
                        Database.AddInParameter(comandoInsertarPersona, "ApellidoMaterno", DbType.String, entity.ApellidoMaterno);
                        Database.AddInParameter(comandoInsertarPersona, "RazonSocial", DbType.String, entity.RazonSocial);
                        Database.AddInParameter(comandoInsertarPersona, "FullName", DbType.String, entity.FullName);
                        Database.AddInParameter(comandoInsertarPersona, "Email", DbType.String, entity.Email);
                        Database.AddInParameter(comandoInsertarPersona, "Fax", DbType.String, entity.Fax);
                        Database.AddInParameter(comandoInsertarPersona, "PaginaWeb", DbType.String, entity.PaginaWeb);
                        Database.AddInParameter(comandoInsertarPersona, "CodigoPostal", DbType.String, entity.CodigoPostal);
                        Database.AddInParameter(comandoInsertarPersona, "ActividadEconomica", DbType.Int32, entity.ActividadEconomica);
                        Database.AddInParameter(comandoInsertarPersona, "NumeroLicencia", DbType.String, entity.NumeroLicencia);
                        Database.AddInParameter(comandoInsertarPersona, "Codigo", DbType.String, entity.Codigo);
                        Database.AddInParameter(comandoInsertarPersona, "FechaCreacion", DbType.DateTime, entity.FechaCreacion);
                        Database.AddInParameter(comandoInsertarPersona, "FechaModificacion", DbType.DateTime, entity.FechaModificacion);
                        Database.AddInParameter(comandoInsertarPersona, "UsuarioCreacion", DbType.Int32, entity.UsuarioCreacion);
                        Database.AddInParameter(comandoInsertarPersona, "UsuarioModificacion", DbType.Int32, entity.UsuarioModificacion);
                        Database.AddInParameter(comandoInsertarPersona, "Estado", DbType.Int32, entity.Estado);
                        Database.AddInParameter(comandoInsertarPersona, "EsExtranjero", DbType.Boolean, entity.EsExtranjero);
                        Database.AddInParameter(comandoInsertarPersona, "Pais", DbType.Int32, entity.Pais);
                        Database.AddInParameter(comandoInsertarPersona, "Direccion", DbType.String, entity.Direccion);
                        Database.AddInParameter(comandoInsertarPersona, "EstadoCivil", DbType.Int32, entity.EstadoCivil);
                        Database.AddInParameter(comandoInsertarPersona, "FechaNacimiento", DbType.DateTime, entity.FechaNacimiento);

                        Database.ExecuteNonQuery(comandoInsertarPersona, transaction);
                        entity.IdPersona = Convert.ToInt32(Database.GetParameterValue(comandoInsertarPersona, "IdPersona"));

                        #endregion

                        #region Funcion
                        
                        var comandoInsertarPersonaFuncion = Database.GetStoredProcCommand("InsertFuncion");
                        Database.AddInParameter(comandoInsertarPersonaFuncion, "IdFuncion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaFuncion, "IdPersona", DbType.Int32);

                        foreach (var personaFuncion in entity.PersonaFunciones)
                        {
                            Database.SetParameterValue(comandoInsertarPersonaFuncion, "IdFuncion", personaFuncion.IdFuncion);
                            Database.SetParameterValue(comandoInsertarPersonaFuncion, "IdPersona", entity.IdPersona);

                            Database.ExecuteNonQuery(comandoInsertarPersonaFuncion, transaction);
                        }

                        #endregion

                        #region Producto
                        if (entity.ProductosProveedor != null)
                        {
                            var comandoInsertarPersonaProducto = Database.GetStoredProcCommand("InsertProductoProveedor");
                            Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPersona", DbType.Int32);
                            Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPresentacion", DbType.Int32);
                            Database.AddInParameter(comandoInsertarPersonaProducto, "pPrecio", DbType.Decimal);

                            foreach (ProductoProveedor proveedorProducto in entity.ProductosProveedor)
                            {
                                Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPersona",
                                                           entity.IdPersona);
                                Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPresentacion",
                                                           proveedorProducto.IdPresentacion);
                                Database.SetParameterValue(comandoInsertarPersonaProducto, "pPrecio",
                                                           proveedorProducto.Precio);

                                var resultado = Database.ExecuteNonQuery(comandoInsertarPersonaProducto, transaction);
                            }
                        }

                        #endregion

                        #region Direccion

                        var comandoPersonaDireccion = Database.GetStoredProcCommand("InsertDireccion");
                        Database.AddOutParameter(comandoPersonaDireccion, "IdDireccion", DbType.Int32, 11);
                        Database.AddInParameter(comandoPersonaDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioModificacion", DbType.Int32);

                        foreach (PersonaDireccion personaDireccion in entity.PersonaDirecciones)
                        {
                            Database.SetParameterValue(comandoPersonaDireccion, "IdPersona", entity.IdPersona);
                            Database.SetParameterValue(comandoPersonaDireccion, "IdUbigeo", personaDireccion.IdUbigeo);
                            Database.SetParameterValue(comandoPersonaDireccion, "Referencia", personaDireccion.Referencia);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoVia", personaDireccion.TipoVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreVia", personaDireccion.NombreVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "Numero", personaDireccion.Numero);
                            Database.SetParameterValue(comandoPersonaDireccion, "Interior", personaDireccion.Interior);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoDireccion", personaDireccion.TipoDireccion);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoZona", personaDireccion.TipoZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreZona", personaDireccion.NombreZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaCreacion", personaDireccion.FechaCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaModificacion", personaDireccion.FechaModificacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioCreacion", personaDireccion.UsuarioCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioModificacion", personaDireccion.UsuarioModificacion);

                            var resultado = Database.ExecuteNonQuery(comandoPersonaDireccion, transaction);
                        }

                        #endregion
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                            throw new Exception(ex.InnerException.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }
          
            return entity;
        }

        public Persona AddC(Persona entity, List<Persona> lista)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Maestro

                        DbCommand cmdInsertarPersona = Database.GetStoredProcCommand("InsertPersona");
                        Database.AddOutParameter(cmdInsertarPersona, "IdPersona", DbType.Int32, 11);
                        Database.AddInParameter(cmdInsertarPersona, "Nombres", DbType.String, entity.Nombres);
                        Database.AddInParameter(cmdInsertarPersona, "TipoPersona", DbType.Int32, entity.TipoPersona);
                        Database.AddInParameter(cmdInsertarPersona, "Documento", DbType.String, entity.Documento);
                        Database.AddInParameter(cmdInsertarPersona, "TipoDocumento", DbType.Int32, entity.TipoPersona);
                        Database.AddInParameter(cmdInsertarPersona, "EsPercepcion", DbType.Byte, entity.TipoDocumento);
                        Database.AddInParameter(cmdInsertarPersona, "Telefono", DbType.String, entity.Telefono);
                        Database.AddInParameter(cmdInsertarPersona, "Celular", DbType.String, entity.Celular);
                        Database.AddInParameter(cmdInsertarPersona, "IdEmpresa", DbType.Int32, entity.IdEmpresa);
                        Database.AddInParameter(cmdInsertarPersona, "ApellidoPaterno", DbType.String, entity.ApellidoPaterno);
                        Database.AddInParameter(cmdInsertarPersona, "ApellidoMaterno", DbType.String, entity.ApellidoMaterno);
                        Database.AddInParameter(cmdInsertarPersona, "RazonSocial", DbType.String, entity.RazonSocial);
                        Database.AddInParameter(cmdInsertarPersona, "FullName", DbType.String, entity.FullName);
                        Database.AddInParameter(cmdInsertarPersona, "Email", DbType.String, entity.Email);
                        Database.AddInParameter(cmdInsertarPersona, "Fax", DbType.String, entity.Fax);
                        Database.AddInParameter(cmdInsertarPersona, "PaginaWeb", DbType.String, entity.PaginaWeb);
                        Database.AddInParameter(cmdInsertarPersona, "CodigoPostal", DbType.String, entity.CodigoPostal);
                        Database.AddInParameter(cmdInsertarPersona, "ActividadEconomica", DbType.Int32, entity.ActividadEconomica);
                        Database.AddInParameter(cmdInsertarPersona, "NumeroLicencia", DbType.String, entity.NumeroLicencia);
                        Database.AddInParameter(cmdInsertarPersona, "Codigo", DbType.String, entity.Codigo);
                        Database.AddInParameter(cmdInsertarPersona, "FechaCreacion", DbType.DateTime, entity.FechaCreacion);
                        Database.AddInParameter(cmdInsertarPersona, "FechaModificacion", DbType.DateTime, entity.FechaModificacion);
                        Database.AddInParameter(cmdInsertarPersona, "UsuarioCreacion", DbType.Int32, entity.UsuarioCreacion);
                        Database.AddInParameter(cmdInsertarPersona, "UsuarioModificacion", DbType.Int32, entity.UsuarioModificacion);
                        Database.AddInParameter(cmdInsertarPersona, "Estado", DbType.Int32, entity.Estado);
                        Database.AddInParameter(cmdInsertarPersona, "EsExtranjero", DbType.Boolean, entity.EsExtranjero);
                        Database.AddInParameter(cmdInsertarPersona, "Pais", DbType.Int32, entity.Pais);
                        Database.AddInParameter(cmdInsertarPersona, "Direccion", DbType.String, entity.Direccion);
                        Database.AddInParameter(cmdInsertarPersona, "EstadoCivil", DbType.Int32, entity.EstadoCivil);
                        Database.AddInParameter(cmdInsertarPersona, "FechaNacimiento", DbType.DateTime, entity.FechaNacimiento);
                        Database.AddInParameter(cmdInsertarPersona, "Sexo", DbType.Byte, entity.Sexo);
                        cmdInsertarPersona.CommandTimeout = 0;

                        Database.ExecuteNonQuery(cmdInsertarPersona, transaction);
                        entity.IdPersona = Convert.ToInt32(Database.GetParameterValue(cmdInsertarPersona, "IdPersona"));

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                            throw new Exception(ex.InnerException.Message);
                        throw new Exception(ex.Message);
                    }
                }
                return entity;
            }
        }

        public List<Persona> GetContactos(int idPersonaPadre)
        {
            var padresContactos = new List<Persona>();
            return padresContactos;
        }

        public bool Insertar(Persona persona, List<Persona> lista)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Persona

                        var comandoInsertarPersona = Database.GetStoredProcCommand("InsertPersona");
                        Database.AddOutParameter(comandoInsertarPersona, "IdPersona", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarPersona, "Nombres", DbType.String, persona.Nombres);
                        Database.AddInParameter(comandoInsertarPersona, "TipoPersona", DbType.Int32, persona.TipoPersona);
                        Database.AddInParameter(comandoInsertarPersona, "Documento", DbType.String, persona.Documento);
                        Database.AddInParameter(comandoInsertarPersona, "TipoDocumento", DbType.Int32, persona.TipoDocumento);
                        Database.AddInParameter(comandoInsertarPersona, "EsPercepcion", DbType.Boolean, persona.EsPercepcion);
                        Database.AddInParameter(comandoInsertarPersona, "Sexo", DbType.Boolean, persona.Sexo);                        
                        Database.AddInParameter(comandoInsertarPersona, "Telefono", DbType.String, persona.Telefono);
                        Database.AddInParameter(comandoInsertarPersona, "Celular", DbType.String, persona.Celular);
                        Database.AddInParameter(comandoInsertarPersona, "IdEmpresa", DbType.Int32, persona.IdEmpresa);
                        Database.AddInParameter(comandoInsertarPersona, "ApellidoPaterno", DbType.String, persona.ApellidoPaterno);
                        Database.AddInParameter(comandoInsertarPersona, "ApellidoMaterno", DbType.String, persona.ApellidoMaterno);
                        Database.AddInParameter(comandoInsertarPersona, "RazonSocial", DbType.String, persona.RazonSocial);
                        Database.AddInParameter(comandoInsertarPersona, "FullName", DbType.String, persona.FullName);
                        Database.AddInParameter(comandoInsertarPersona, "Email", DbType.String, persona.Email);
                        Database.AddInParameter(comandoInsertarPersona, "Fax", DbType.String, persona.Fax);
                        Database.AddInParameter(comandoInsertarPersona, "PaginaWeb", DbType.String, persona.PaginaWeb);
                        Database.AddInParameter(comandoInsertarPersona, "CodigoPostal", DbType.String, persona.CodigoPostal);
                        Database.AddInParameter(comandoInsertarPersona, "ActividadEconomica", DbType.Int32, persona.ActividadEconomica);
                        Database.AddInParameter(comandoInsertarPersona, "NumeroLicencia", DbType.String, persona.NumeroLicencia);
                        Database.AddInParameter(comandoInsertarPersona, "Codigo", DbType.String, persona.Codigo);
                        Database.AddInParameter(comandoInsertarPersona, "FechaCreacion", DbType.DateTime, persona.FechaCreacion);
                        Database.AddInParameter(comandoInsertarPersona, "FechaModificacion", DbType.DateTime, persona.FechaModificacion);
                        Database.AddInParameter(comandoInsertarPersona, "UsuarioCreacion", DbType.Int32, persona.UsuarioCreacion);
                        Database.AddInParameter(comandoInsertarPersona, "UsuarioModificacion", DbType.Int32, persona.UsuarioModificacion);
                        Database.AddInParameter(comandoInsertarPersona, "Estado", DbType.Int32, persona.Estado);
                        Database.AddInParameter(comandoInsertarPersona, "EsExtranjero", DbType.Boolean, persona.EsExtranjero);
                        Database.AddInParameter(comandoInsertarPersona, "Pais", DbType.Int32, persona.Pais);
                        Database.AddInParameter(comandoInsertarPersona, "Direccion", DbType.String, persona.Direccion);
                        Database.AddInParameter(comandoInsertarPersona, "EstadoCivil", DbType.Int32, persona.EstadoCivil);
                        Database.AddInParameter(comandoInsertarPersona, "FechaNacimiento", DbType.DateTime, persona.FechaNacimiento);

                        Database.ExecuteNonQuery(comandoInsertarPersona, transaction);
                        persona.IdPersona = Convert.ToInt32(Database.GetParameterValue(comandoInsertarPersona, "IdPersona"));

                        #endregion

                        #region Funcion

                        var comandoInsertarPersonaFuncion = Database.GetStoredProcCommand("InsertFuncion");
                        Database.AddInParameter(comandoInsertarPersonaFuncion, "IdFuncion", DbType.Int32, persona.PersonaFunciones.First().IdFuncion);
                        Database.AddInParameter(comandoInsertarPersonaFuncion, "IdPersona", DbType.Int32, persona.IdPersona);
                        Database.ExecuteNonQuery(comandoInsertarPersonaFuncion, transaction);

                        #endregion

                        #region Producto

                        var comandoInsertarPersonaProducto = Database.GetStoredProcCommand("InsertProductoProveedor");
                        Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPersona", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPresentacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaProducto, "pPrecio", DbType.Decimal);

                        foreach (ProductoProveedor proveedorProducto in persona.ProductosProveedor)
                        {
                            Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPersona", persona.IdPersona);
                            Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPresentacion", proveedorProducto.IdPresentacion);
                            Database.SetParameterValue(comandoInsertarPersonaProducto, "pPrecio", proveedorProducto.Precio);

                            var resultado = Database.ExecuteNonQuery(comandoInsertarPersonaProducto, transaction);
                        }

                        #endregion

                        #region Direccion

                        var comandoPersonaDireccion = Database.GetStoredProcCommand("InsertDireccion");
                        Database.AddOutParameter(comandoPersonaDireccion, "IdDireccion", DbType.Int32,11);
                        Database.AddInParameter(comandoPersonaDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioModificacion", DbType.Int32);

                        foreach (PersonaDireccion personaDireccion in persona.PersonaDirecciones)
                        {
                            Database.SetParameterValue(comandoPersonaDireccion, "IdPersona", persona.IdPersona);
                            Database.SetParameterValue(comandoPersonaDireccion, "IdUbigeo", personaDireccion.IdUbigeo);
                            Database.SetParameterValue(comandoPersonaDireccion, "Referencia", personaDireccion.Referencia);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoVia", personaDireccion.TipoVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreVia", personaDireccion.NombreVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "Numero", personaDireccion.Numero);
                            Database.SetParameterValue(comandoPersonaDireccion, "Interior", personaDireccion.Interior);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoDireccion", personaDireccion.TipoDireccion);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoZona", personaDireccion.TipoZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreZona", personaDireccion.NombreZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaCreacion", personaDireccion.FechaCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaModificacion", personaDireccion.FechaModificacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioCreacion", personaDireccion.UsuarioCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioModificacion", personaDireccion.UsuarioModificacion);

                            var resultado = Database.ExecuteNonQuery(comandoPersonaDireccion, transaction);
                        }

                        #endregion

                        #region Contacto

                        #region Parametros Persona

                        var comandoInsertarPContacto = Database.GetStoredProcCommand("InsertPersona");
                        Database.AddOutParameter(comandoInsertarPContacto, "IdPersona", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarPContacto, "Nombres", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "TipoPersona", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "Documento", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "TipoDocumento", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "EsPercepcion", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPContacto, "Sexo", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPContacto, "Telefono", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Celular", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "IdEmpresa", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "ApellidoPaterno", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "ApellidoMaterno", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "RazonSocial", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "FullName", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Email", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Fax", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "PaginaWeb", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "CodigoPostal", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "ActividadEconomica", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "NumeroLicencia", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Codigo", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPContacto, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPContacto, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "UsuarioModificacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "Estado", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "EsExtranjero", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPContacto, "Pais", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "Direccion", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "EstadoCivil", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "FechaNacimiento", DbType.DateTime);

                        #endregion

                        #region Parametros Funcion

                        var comandoInsertarPersonaCFuncion = Database.GetStoredProcCommand("InsertFuncion");
                        Database.AddInParameter(comandoInsertarPersonaCFuncion, "IdFuncion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaCFuncion, "IdPersona", DbType.Int32);

                        #endregion

                        #region Parametros Direccion

                        var comandoPersonaCDireccion = Database.GetStoredProcCommand("InsertDireccion");
                        Database.AddOutParameter(comandoPersonaCDireccion, "IdDireccion", DbType.Int32, 11);
                        Database.AddInParameter(comandoPersonaCDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaCDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaCDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "UsuarioModificacion", DbType.Int32);

                        #endregion

                        #region Parametros Contacto

                        var comandoInsertarPersonaContacto = Database.GetStoredProcCommand("InsertContacto");
                        Database.AddOutParameter(comandoInsertarPersonaContacto, "IdPersonaContacto", DbType.Int32, 11);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "IdContacto", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "Cargo", DbType.String);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "TipoContacto", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "Codigo", DbType.String);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "UsuarioModificacion", DbType.Int32);

                        #endregion

                        foreach (Persona personaC in lista)
                        {
                            #region Persona
                            
                            Database.SetParameterValue(comandoInsertarPContacto, "Nombres", personaC.Nombres);
                            Database.SetParameterValue(comandoInsertarPContacto, "TipoPersona", personaC.TipoPersona);
                            Database.SetParameterValue(comandoInsertarPContacto, "Documento", personaC.Documento);
                            Database.SetParameterValue(comandoInsertarPContacto, "TipoDocumento", personaC.TipoDocumento);
                            Database.SetParameterValue(comandoInsertarPContacto, "EsPercepcion", personaC.EsPercepcion);
                            Database.SetParameterValue(comandoInsertarPContacto, "Sexo", personaC.Sexo);
                            Database.SetParameterValue(comandoInsertarPContacto, "Telefono", personaC.Telefono);
                            Database.SetParameterValue(comandoInsertarPContacto, "Celular", personaC.Celular);
                            Database.SetParameterValue(comandoInsertarPContacto, "IdEmpresa", personaC.IdEmpresa);
                            Database.SetParameterValue(comandoInsertarPContacto, "ApellidoPaterno", personaC.ApellidoPaterno);
                            Database.SetParameterValue(comandoInsertarPContacto, "ApellidoMaterno", personaC.ApellidoMaterno);
                            Database.SetParameterValue(comandoInsertarPContacto, "RazonSocial", personaC.RazonSocial);
                            Database.SetParameterValue(comandoInsertarPContacto, "FullName", personaC.FullName);
                            Database.SetParameterValue(comandoInsertarPContacto, "Email", personaC.Email);
                            Database.SetParameterValue(comandoInsertarPContacto, "Fax", personaC.Fax);
                            Database.SetParameterValue(comandoInsertarPContacto, "PaginaWeb", personaC.PaginaWeb);
                            Database.SetParameterValue(comandoInsertarPContacto, "CodigoPostal", personaC.CodigoPostal);
                            Database.SetParameterValue(comandoInsertarPContacto, "ActividadEconomica", personaC.ActividadEconomica);
                            Database.SetParameterValue(comandoInsertarPContacto, "NumeroLicencia", personaC.NumeroLicencia);
                            Database.SetParameterValue(comandoInsertarPContacto, "Codigo", personaC.Codigo);
                            Database.SetParameterValue(comandoInsertarPContacto, "FechaCreacion", personaC.FechaCreacion);
                            Database.SetParameterValue(comandoInsertarPContacto, "FechaModificacion", personaC.FechaModificacion);
                            Database.SetParameterValue(comandoInsertarPContacto, "UsuarioCreacion", personaC.UsuarioCreacion);
                            Database.SetParameterValue(comandoInsertarPContacto, "UsuarioModificacion", personaC.UsuarioModificacion);
                            Database.SetParameterValue(comandoInsertarPContacto, "Estado", personaC.Estado);
                            Database.SetParameterValue(comandoInsertarPContacto, "EsExtranjero", personaC.EsExtranjero);
                            Database.SetParameterValue(comandoInsertarPContacto, "Pais", personaC.Pais);
                            Database.SetParameterValue(comandoInsertarPContacto, "Direccion", personaC.Direccion);
                            Database.SetParameterValue(comandoInsertarPContacto, "EstadoCivil", personaC.EstadoCivil);
                            Database.SetParameterValue(comandoInsertarPContacto, "FechaNacimiento", personaC.FechaNacimiento);

                            Database.ExecuteNonQuery(comandoInsertarPContacto, transaction);
                            personaC.IdPersona = Convert.ToInt32(Database.GetParameterValue(comandoInsertarPContacto, "IdPersona"));

                            #endregion

                            #region Funcion

                            Database.SetParameterValue(comandoInsertarPersonaCFuncion, "IdFuncion", personaC.PersonaFunciones.First().IdFuncion);
                            Database.SetParameterValue(comandoInsertarPersonaCFuncion, "IdPersona", personaC.IdPersona);
                            Database.ExecuteNonQuery(comandoInsertarPersonaCFuncion, transaction);

                            #endregion

                            #region Direccion

                            Database.SetParameterValue(comandoPersonaCDireccion, "IdPersona", personaC.IdPersona);
                            Database.SetParameterValue(comandoPersonaCDireccion, "IdUbigeo", personaC.PersonaDirecciones.First().IdUbigeo);
                            Database.SetParameterValue(comandoPersonaCDireccion, "Referencia", personaC.PersonaDirecciones.First().Referencia);
                            Database.SetParameterValue(comandoPersonaCDireccion, "TipoVia", personaC.PersonaDirecciones.First().TipoVia);
                            Database.SetParameterValue(comandoPersonaCDireccion, "NombreVia", personaC.PersonaDirecciones.First().NombreVia);
                            Database.SetParameterValue(comandoPersonaCDireccion, "Numero", personaC.PersonaDirecciones.First().Numero);
                            Database.SetParameterValue(comandoPersonaCDireccion, "Interior", personaC.PersonaDirecciones.First().Interior);
                            Database.SetParameterValue(comandoPersonaCDireccion, "TipoDireccion", personaC.PersonaDirecciones.First().TipoDireccion);
                            Database.SetParameterValue(comandoPersonaCDireccion, "TipoZona", personaC.PersonaDirecciones.First().TipoZona);
                            Database.SetParameterValue(comandoPersonaCDireccion, "NombreZona", personaC.PersonaDirecciones.First().NombreZona);
                            Database.SetParameterValue(comandoPersonaCDireccion, "FechaCreacion", personaC.PersonaDirecciones.First().FechaCreacion);
                            Database.SetParameterValue(comandoPersonaCDireccion, "FechaModificacion", personaC.PersonaDirecciones.First().FechaModificacion);
                            Database.SetParameterValue(comandoPersonaCDireccion, "UsuarioCreacion", personaC.PersonaDirecciones.First().UsuarioCreacion);
                            Database.SetParameterValue(comandoPersonaCDireccion, "UsuarioModificacion", personaC.PersonaDirecciones.First().UsuarioModificacion);
                            Database.ExecuteNonQuery(comandoPersonaCDireccion, transaction);

                            #endregion

                            #region Contacto

                            Database.SetParameterValue(comandoInsertarPersonaContacto, "IdPersona", persona.IdPersona);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "IdContacto", personaC.IdPersona);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "Cargo", personaC.PersonaContactos1.First().Cargo);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "TipoContacto", personaC.PersonaContactos1.First().TipoContacto);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "FechaCreacion", personaC.PersonaContactos1.First().FechaCreacion);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "FechaModificacion", personaC.PersonaContactos1.First().FechaModificacion);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "UsuarioCreacion", personaC.PersonaContactos1.First().UsuarioCreacion);
                            Database.SetParameterValue(comandoInsertarPersonaContacto, "UsuarioModificacion", personaC.PersonaContactos1.First().UsuarioModificacion);

                            Database.ExecuteNonQuery(comandoInsertarPersonaContacto, transaction);

                            #endregion

                        }                        

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                            throw new Exception(ex.InnerException.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }
            return true;
        }

        public bool Actualizar(Persona persona, List<Persona> lista)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Persona

                        var comandoActualizarPersona = Database.GetStoredProcCommand("UpdatePersona");
                        Database.AddInParameter(comandoActualizarPersona, "IdPersona", DbType.Int32, persona.IdPersona);
                        Database.AddInParameter(comandoActualizarPersona, "Nombres", DbType.String, persona.Nombres);
                        Database.AddInParameter(comandoActualizarPersona, "TipoPersona", DbType.Int32, persona.TipoPersona);
                        Database.AddInParameter(comandoActualizarPersona, "Documento", DbType.String, persona.Documento);
                        Database.AddInParameter(comandoActualizarPersona, "TipoDocumento", DbType.Int32, persona.TipoDocumento);
                        Database.AddInParameter(comandoActualizarPersona, "EsPercepcion", DbType.Boolean, persona.EsPercepcion);
                        Database.AddInParameter(comandoActualizarPersona, "Sexo", DbType.Boolean, persona.Sexo);
                        Database.AddInParameter(comandoActualizarPersona, "Telefono", DbType.String, persona.Telefono);
                        Database.AddInParameter(comandoActualizarPersona, "Celular", DbType.String, persona.Celular);
                        Database.AddInParameter(comandoActualizarPersona, "IdEmpresa", DbType.Int32, persona.IdEmpresa);
                        Database.AddInParameter(comandoActualizarPersona, "ApellidoPaterno", DbType.String, persona.ApellidoPaterno);
                        Database.AddInParameter(comandoActualizarPersona, "ApellidoMaterno", DbType.String, persona.ApellidoMaterno);
                        Database.AddInParameter(comandoActualizarPersona, "RazonSocial", DbType.String, persona.RazonSocial);
                        Database.AddInParameter(comandoActualizarPersona, "FullName", DbType.String, persona.FullName);
                        Database.AddInParameter(comandoActualizarPersona, "Email", DbType.String, persona.Email);
                        Database.AddInParameter(comandoActualizarPersona, "Fax", DbType.String, persona.Fax);
                        Database.AddInParameter(comandoActualizarPersona, "PaginaWeb", DbType.String, persona.PaginaWeb);
                        Database.AddInParameter(comandoActualizarPersona, "CodigoPostal", DbType.String, persona.CodigoPostal);
                        Database.AddInParameter(comandoActualizarPersona, "ActividadEconomica", DbType.Int32, persona.ActividadEconomica);
                        Database.AddInParameter(comandoActualizarPersona, "NumeroLicencia", DbType.String, persona.NumeroLicencia);
                        Database.AddInParameter(comandoActualizarPersona, "Codigo", DbType.String, persona.Codigo);
                        Database.AddInParameter(comandoActualizarPersona, "FechaCreacion", DbType.DateTime, persona.FechaCreacion);
                        Database.AddInParameter(comandoActualizarPersona, "FechaModificacion", DbType.DateTime, persona.FechaModificacion);
                        Database.AddInParameter(comandoActualizarPersona, "UsuarioCreacion", DbType.Int32, persona.UsuarioCreacion);
                        Database.AddInParameter(comandoActualizarPersona, "UsuarioModificacion", DbType.Int32, persona.UsuarioModificacion);
                        Database.AddInParameter(comandoActualizarPersona, "Estado", DbType.Int32, persona.Estado);
                        Database.AddInParameter(comandoActualizarPersona, "EsExtranjero", DbType.Boolean, persona.EsExtranjero);
                        Database.AddInParameter(comandoActualizarPersona, "Pais", DbType.Int32, persona.Pais);
                        Database.AddInParameter(comandoActualizarPersona, "Direccion", DbType.String, persona.Direccion);
                        Database.AddInParameter(comandoActualizarPersona, "EstadoCivil", DbType.Int32, persona.EstadoCivil);
                        Database.AddInParameter(comandoActualizarPersona, "FechaNacimiento", DbType.DateTime, persona.FechaNacimiento);

                        Database.ExecuteNonQuery(comandoActualizarPersona, transaction);

                        #endregion

                        #region Funcion

                        var comandoActualizarPersonaFuncion = Database.GetStoredProcCommand("UpdateFuncion");
                        Database.AddInParameter(comandoActualizarPersonaFuncion, "IdFuncion", DbType.Int32, persona.PersonaFunciones.First().IdFuncion);
                        Database.AddInParameter(comandoActualizarPersonaFuncion, "IdPersona", DbType.Int32, persona.IdPersona);
                        Database.ExecuteNonQuery(comandoActualizarPersonaFuncion, transaction);

                        #endregion

                        #region Producto

                        #region ComandoInsertar

                        var comandoInsertarPersonaProducto = Database.GetStoredProcCommand("InsertProductoProveedor");
                        Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPersona", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaProducto, "pIdPresentacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaProducto, "pPrecio", DbType.Decimal);

                        foreach (ProductoProveedor proveedorProducto in persona.ProductosProveedor)
                        {
                            Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPersona", persona.IdPersona);
                            Database.SetParameterValue(comandoInsertarPersonaProducto, "pIdPresentacion", proveedorProducto.IdPresentacion);
                            Database.SetParameterValue(comandoInsertarPersonaProducto, "pPrecio", proveedorProducto.Precio);

                            var resultado = Database.ExecuteNonQuery(comandoInsertarPersonaProducto, transaction);
                        }

                        #endregion

                        #endregion

                        #region Direccion

                        #region ComandoEliminar

                        var comandoDeletePersonaDireccion = Database.GetStoredProcCommand("DeleteDireccion");
                        Database.AddInParameter(comandoDeletePersonaDireccion, "IdPersona", DbType.Int32, persona.IdPersona);
                        Database.ExecuteNonQuery(comandoDeletePersonaDireccion, transaction);

                        #endregion

                        #region ComandoInsertar

                        var comandoPersonaDireccion = Database.GetStoredProcCommand("InsertDireccion");
                        Database.AddOutParameter(comandoPersonaDireccion, "IdDireccion", DbType.Int32, 11);
                        Database.AddInParameter(comandoPersonaDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaDireccion, "UsuarioModificacion", DbType.Int32);

                        foreach (PersonaDireccion personaDireccion in persona.PersonaDirecciones)
                        {
                            Database.SetParameterValue(comandoPersonaDireccion, "IdPersona", persona.IdPersona);
                            Database.SetParameterValue(comandoPersonaDireccion, "IdUbigeo", personaDireccion.IdUbigeo);
                            Database.SetParameterValue(comandoPersonaDireccion, "Referencia", personaDireccion.Referencia);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoVia", personaDireccion.TipoVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreVia", personaDireccion.NombreVia);
                            Database.SetParameterValue(comandoPersonaDireccion, "Numero", personaDireccion.Numero);
                            Database.SetParameterValue(comandoPersonaDireccion, "Interior", personaDireccion.Interior);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoDireccion", personaDireccion.TipoDireccion);
                            Database.SetParameterValue(comandoPersonaDireccion, "TipoZona", personaDireccion.TipoZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "NombreZona", personaDireccion.NombreZona);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaCreacion", personaDireccion.FechaCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "FechaModificacion", personaDireccion.FechaModificacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioCreacion", personaDireccion.UsuarioCreacion);
                            Database.SetParameterValue(comandoPersonaDireccion, "UsuarioModificacion", personaDireccion.UsuarioModificacion);

                            Database.ExecuteNonQuery(comandoPersonaDireccion, transaction);
                        }

                        #endregion

                        #endregion

                        #region Contacto

                        #region Parametros Persona

                        #region ComandoEliminar

                        var comandoDeletePContacto = Database.GetStoredProcCommand("DeletePersona");
                        Database.AddInParameter(comandoDeletePContacto, "IdPersona", DbType.Int32);                        

                        #endregion

                        #region ComandoInsertar

                        var comandoInsertarPContacto = Database.GetStoredProcCommand("InsertPersona");
                        Database.AddOutParameter(comandoInsertarPContacto, "IdPersona", DbType.Int32, 0);
                        Database.AddInParameter(comandoInsertarPContacto, "Nombres", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "TipoPersona", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "Documento", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "TipoDocumento", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "EsPercepcion", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPContacto, "Sexo", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPContacto, "Telefono", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Celular", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "IdEmpresa", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "ApellidoPaterno", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "ApellidoMaterno", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "RazonSocial", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "FullName", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Email", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Fax", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "PaginaWeb", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "CodigoPostal", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "ActividadEconomica", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "NumeroLicencia", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "Codigo", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPContacto, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPContacto, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "UsuarioModificacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "Estado", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "EsExtranjero", DbType.Boolean);
                        Database.AddInParameter(comandoInsertarPContacto, "Pais", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "Direccion", DbType.String);
                        Database.AddInParameter(comandoInsertarPContacto, "EstadoCivil", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPContacto, "FechaNacimiento", DbType.DateTime);

                        #endregion

                        #region ComandoActualizar

                        var comandoActualizarPContacto = Database.GetStoredProcCommand("UpdatePersona");
                        Database.AddInParameter(comandoActualizarPContacto, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "Nombres", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "TipoPersona", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "Documento", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "TipoDocumento", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "EsPercepcion", DbType.Boolean);
                        Database.AddInParameter(comandoActualizarPContacto, "Sexo", DbType.Boolean);
                        Database.AddInParameter(comandoActualizarPContacto, "Telefono", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "Celular", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "IdEmpresa", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "ApellidoPaterno", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "ApellidoMaterno", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "RazonSocial", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "FullName", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "Email", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "Fax", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "PaginaWeb", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "CodigoPostal", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "ActividadEconomica", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "NumeroLicencia", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "Codigo", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoActualizarPContacto, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoActualizarPContacto, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "UsuarioModificacion", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "Estado", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "EsExtranjero", DbType.Boolean);
                        Database.AddInParameter(comandoActualizarPContacto, "Pais", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "Direccion", DbType.String);
                        Database.AddInParameter(comandoActualizarPContacto, "EstadoCivil", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPContacto, "FechaNacimiento", DbType.DateTime);

                        #endregion

                        #endregion

                        #region Parametros Funcion

                        var comandoActualizarPersonaCFuncion = Database.GetStoredProcCommand("UpdateFuncion");
                        Database.AddInParameter(comandoActualizarPersonaCFuncion, "IdFuncion", DbType.Int32);
                        Database.AddInParameter(comandoActualizarPersonaCFuncion, "IdPersona", DbType.Int32);

                        #endregion

                        #region Parametros Direccion

                        #region ComandoInsertar

                        var comandoPersonaCDireccion = Database.GetStoredProcCommand("InsertDireccion");
                        Database.AddOutParameter(comandoPersonaCDireccion, "IdDireccion", DbType.Int32, 11);
                        Database.AddInParameter(comandoPersonaCDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoPersonaCDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaCDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoPersonaCDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoPersonaCDireccion, "UsuarioModificacion", DbType.Int32);

                        #endregion

                        #region ComandoModificar

                        var comandoModificarPersonaCDireccion = Database.GetStoredProcCommand("UpdateDireccion");
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "IdDireccion", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "IdUbigeo", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "Referencia", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "TipoVia", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "NombreVia", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "Numero", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "Interior", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "TipoDireccion", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "TipoZona", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "NombreZona", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaCDireccion, "UsuarioModificacion", DbType.Int32);

                        #endregion

                        #endregion

                        #region Parametros Contacto

                        #region ComandoInsertar

                        var comandoInsertarPersonaContacto = Database.GetStoredProcCommand("InsertContacto");
                        Database.AddOutParameter(comandoInsertarPersonaContacto, "IdPersonaContacto", DbType.Int32, 11);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "IdContacto", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "Cargo", DbType.String);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "TipoContacto", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "Codigo", DbType.String);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoInsertarPersonaContacto, "UsuarioModificacion", DbType.Int32);

                        #endregion

                        #region ComandoModificar

                        var comandoModificarPersonaContacto = Database.GetStoredProcCommand("UpdateContacto");
                        Database.AddInParameter(comandoModificarPersonaContacto, "IdPersonaContacto", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaContacto, "IdPersona", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaContacto, "IdContacto", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaContacto, "Cargo", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaContacto, "TipoContacto", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaContacto, "Codigo", DbType.String);
                        Database.AddInParameter(comandoModificarPersonaContacto, "FechaCreacion", DbType.DateTime);
                        Database.AddInParameter(comandoModificarPersonaContacto, "FechaModificacion", DbType.DateTime);
                        Database.AddInParameter(comandoModificarPersonaContacto, "UsuarioCreacion", DbType.Int32);
                        Database.AddInParameter(comandoModificarPersonaContacto, "UsuarioModificacion", DbType.Int32);

                        #endregion

                        #endregion

                        foreach (Persona personaC in lista)
                        {
                            #region Persona

                            if (personaC.IdPersona == 0)
                            {
                                Database.SetParameterValue(comandoInsertarPContacto, "Nombres", personaC.Nombres);
                                Database.SetParameterValue(comandoInsertarPContacto, "TipoPersona", personaC.TipoPersona);
                                Database.SetParameterValue(comandoInsertarPContacto, "Documento", personaC.Documento);
                                Database.SetParameterValue(comandoInsertarPContacto, "TipoDocumento", personaC.TipoDocumento);
                                Database.SetParameterValue(comandoInsertarPContacto, "EsPercepcion", personaC.EsPercepcion);
                                Database.SetParameterValue(comandoInsertarPContacto, "Sexo", personaC.Sexo);
                                Database.SetParameterValue(comandoInsertarPContacto, "Telefono", personaC.Telefono);
                                Database.SetParameterValue(comandoInsertarPContacto, "Celular", personaC.Celular);
                                Database.SetParameterValue(comandoInsertarPContacto, "IdEmpresa", personaC.IdEmpresa);
                                Database.SetParameterValue(comandoInsertarPContacto, "ApellidoPaterno", personaC.ApellidoPaterno);
                                Database.SetParameterValue(comandoInsertarPContacto, "ApellidoMaterno", personaC.ApellidoMaterno);
                                Database.SetParameterValue(comandoInsertarPContacto, "RazonSocial", personaC.RazonSocial);
                                Database.SetParameterValue(comandoInsertarPContacto, "FullName", personaC.FullName);
                                Database.SetParameterValue(comandoInsertarPContacto, "Email", personaC.Email);
                                Database.SetParameterValue(comandoInsertarPContacto, "Fax", personaC.Fax);
                                Database.SetParameterValue(comandoInsertarPContacto, "PaginaWeb", personaC.PaginaWeb);
                                Database.SetParameterValue(comandoInsertarPContacto, "CodigoPostal", personaC.CodigoPostal);
                                Database.SetParameterValue(comandoInsertarPContacto, "ActividadEconomica", personaC.ActividadEconomica);
                                Database.SetParameterValue(comandoInsertarPContacto, "NumeroLicencia", personaC.NumeroLicencia);
                                Database.SetParameterValue(comandoInsertarPContacto, "Codigo", personaC.Codigo);
                                Database.SetParameterValue(comandoInsertarPContacto, "FechaCreacion", personaC.FechaCreacion);
                                Database.SetParameterValue(comandoInsertarPContacto, "FechaModificacion", personaC.FechaModificacion);
                                Database.SetParameterValue(comandoInsertarPContacto, "UsuarioCreacion", personaC.UsuarioCreacion);
                                Database.SetParameterValue(comandoInsertarPContacto, "UsuarioModificacion", personaC.UsuarioModificacion);
                                Database.SetParameterValue(comandoInsertarPContacto, "Estado", personaC.Estado);
                                Database.SetParameterValue(comandoInsertarPContacto, "EsExtranjero", personaC.EsExtranjero);
                                Database.SetParameterValue(comandoInsertarPContacto, "Pais", personaC.Pais);
                                Database.SetParameterValue(comandoInsertarPContacto, "Direccion", personaC.Direccion);
                                Database.SetParameterValue(comandoInsertarPContacto, "EstadoCivil", personaC.EstadoCivil);
                                Database.SetParameterValue(comandoInsertarPContacto, "FechaNacimiento", personaC.FechaNacimiento);

                                Database.ExecuteNonQuery(comandoInsertarPContacto, transaction);
                                personaC.IdPersona = Convert.ToInt32(Database.GetParameterValue(comandoInsertarPContacto, "IdPersona"));

                                #region Funcion

                                var comandoInsertarPersonaFuncion = Database.GetStoredProcCommand("InsertFuncion");
                                Database.AddInParameter(comandoInsertarPersonaFuncion, "IdFuncion", DbType.Int32, personaC.PersonaFunciones.First().IdFuncion);
                                Database.AddInParameter(comandoInsertarPersonaFuncion, "IdPersona", DbType.Int32, personaC.IdPersona);
                                Database.ExecuteNonQuery(comandoInsertarPersonaFuncion, transaction);

                                #endregion

                                #region Direccion

                                Database.SetParameterValue(comandoPersonaCDireccion, "IdPersona", personaC.IdPersona);
                                Database.SetParameterValue(comandoPersonaCDireccion, "IdUbigeo", personaC.PersonaDirecciones.First().IdUbigeo);
                                Database.SetParameterValue(comandoPersonaCDireccion, "Referencia", personaC.PersonaDirecciones.First().Referencia);
                                Database.SetParameterValue(comandoPersonaCDireccion, "TipoVia", personaC.PersonaDirecciones.First().TipoVia);
                                Database.SetParameterValue(comandoPersonaCDireccion, "NombreVia", personaC.PersonaDirecciones.First().NombreVia);
                                Database.SetParameterValue(comandoPersonaCDireccion, "Numero", personaC.PersonaDirecciones.First().Numero);
                                Database.SetParameterValue(comandoPersonaCDireccion, "Interior", personaC.PersonaDirecciones.First().Interior);
                                Database.SetParameterValue(comandoPersonaCDireccion, "TipoDireccion", personaC.PersonaDirecciones.First().TipoDireccion);
                                Database.SetParameterValue(comandoPersonaCDireccion, "TipoZona", personaC.PersonaDirecciones.First().TipoZona);
                                Database.SetParameterValue(comandoPersonaCDireccion, "NombreZona", personaC.PersonaDirecciones.First().NombreZona);
                                Database.SetParameterValue(comandoPersonaCDireccion, "FechaCreacion", personaC.PersonaDirecciones.First().FechaCreacion);
                                Database.SetParameterValue(comandoPersonaCDireccion, "FechaModificacion", personaC.PersonaDirecciones.First().FechaModificacion);
                                Database.SetParameterValue(comandoPersonaCDireccion, "UsuarioCreacion", personaC.PersonaDirecciones.First().UsuarioCreacion);
                                Database.SetParameterValue(comandoPersonaCDireccion, "UsuarioModificacion", personaC.PersonaDirecciones.First().UsuarioModificacion);
                                Database.ExecuteNonQuery(comandoPersonaCDireccion, transaction);

                                #endregion

                                #region Contacto

                                Database.SetParameterValue(comandoInsertarPersonaContacto, "IdPersona", persona.IdPersona);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "IdContacto", personaC.IdPersona);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "Cargo", personaC.PersonaContactos1.First().Cargo);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "TipoContacto", personaC.PersonaContactos1.First().TipoContacto);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "FechaCreacion", personaC.PersonaContactos1.First().FechaCreacion);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "FechaModificacion", personaC.PersonaContactos1.First().FechaModificacion);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "UsuarioCreacion", personaC.PersonaContactos1.First().UsuarioCreacion);
                                Database.SetParameterValue(comandoInsertarPersonaContacto, "UsuarioModificacion", personaC.PersonaContactos1.First().UsuarioModificacion);

                                Database.ExecuteNonQuery(comandoInsertarPersonaContacto, transaction);

                                #endregion

                            }
                            else
                            {
                                Database.SetParameterValue(comandoActualizarPContacto, "Nombres", personaC.Nombres);
                                Database.SetParameterValue(comandoActualizarPContacto, "TipoPersona", personaC.TipoPersona);
                                Database.SetParameterValue(comandoActualizarPContacto, "Documento", personaC.Documento);
                                Database.SetParameterValue(comandoActualizarPContacto, "TipoDocumento", personaC.TipoDocumento);
                                Database.SetParameterValue(comandoActualizarPContacto, "EsPercepcion", personaC.EsPercepcion);
                                Database.SetParameterValue(comandoActualizarPContacto, "Sexo", personaC.Sexo);
                                Database.SetParameterValue(comandoActualizarPContacto, "Telefono", personaC.Telefono);
                                Database.SetParameterValue(comandoActualizarPContacto, "Celular", personaC.Celular);
                                Database.SetParameterValue(comandoActualizarPContacto, "IdEmpresa", personaC.IdEmpresa);
                                Database.SetParameterValue(comandoActualizarPContacto, "ApellidoPaterno", personaC.ApellidoPaterno);
                                Database.SetParameterValue(comandoActualizarPContacto, "ApellidoMaterno", personaC.ApellidoMaterno);
                                Database.SetParameterValue(comandoActualizarPContacto, "RazonSocial", personaC.RazonSocial);
                                Database.SetParameterValue(comandoActualizarPContacto, "FullName", personaC.FullName);
                                Database.SetParameterValue(comandoActualizarPContacto, "Email", personaC.Email);
                                Database.SetParameterValue(comandoActualizarPContacto, "Fax", personaC.Fax);
                                Database.SetParameterValue(comandoActualizarPContacto, "PaginaWeb", personaC.PaginaWeb);
                                Database.SetParameterValue(comandoActualizarPContacto, "CodigoPostal", personaC.CodigoPostal);
                                Database.SetParameterValue(comandoActualizarPContacto, "ActividadEconomica", personaC.ActividadEconomica);
                                Database.SetParameterValue(comandoActualizarPContacto, "NumeroLicencia", personaC.NumeroLicencia);
                                Database.SetParameterValue(comandoActualizarPContacto, "Codigo", personaC.Codigo);
                                Database.SetParameterValue(comandoActualizarPContacto, "FechaCreacion", personaC.FechaCreacion);
                                Database.SetParameterValue(comandoActualizarPContacto, "FechaModificacion", personaC.FechaModificacion);
                                Database.SetParameterValue(comandoActualizarPContacto, "UsuarioCreacion", personaC.UsuarioCreacion);
                                Database.SetParameterValue(comandoActualizarPContacto, "UsuarioModificacion", personaC.UsuarioModificacion);
                                Database.SetParameterValue(comandoActualizarPContacto, "Estado", personaC.Estado);
                                Database.SetParameterValue(comandoActualizarPContacto, "EsExtranjero", personaC.EsExtranjero);
                                Database.SetParameterValue(comandoActualizarPContacto, "Pais", personaC.Pais);
                                Database.SetParameterValue(comandoActualizarPContacto, "Direccion", personaC.Direccion);
                                Database.SetParameterValue(comandoActualizarPContacto, "EstadoCivil", personaC.EstadoCivil);
                                Database.SetParameterValue(comandoActualizarPContacto, "FechaNacimiento", personaC.FechaNacimiento);

                                Database.ExecuteNonQuery(comandoActualizarPContacto, transaction);

                                #region Funcion

                                Database.SetParameterValue(comandoActualizarPersonaCFuncion, "IdFuncion", personaC.PersonaFunciones.First().IdFuncion);
                                Database.SetParameterValue(comandoActualizarPersonaCFuncion, "IdPersona", personaC.IdPersona);
                                Database.ExecuteNonQuery(comandoActualizarPersonaCFuncion, transaction);

                                #endregion

                                #region Direccion

                                #region ComandoModificar

                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "IdPersona", personaC.IdPersona);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "IdDireccion", personaC.PersonaDirecciones.First().IdDireccion);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "IdUbigeo", personaC.PersonaDirecciones.First().IdUbigeo);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "Referencia", personaC.PersonaDirecciones.First().Referencia);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "TipoVia", personaC.PersonaDirecciones.First().TipoVia);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "NombreVia", personaC.PersonaDirecciones.First().NombreVia);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "Numero", personaC.PersonaDirecciones.First().Numero);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "Interior", personaC.PersonaDirecciones.First().Interior);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "TipoDireccion", personaC.PersonaDirecciones.First().TipoDireccion);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "TipoZona", personaC.PersonaDirecciones.First().TipoZona);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "NombreZona", personaC.PersonaDirecciones.First().NombreZona);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "FechaCreacion", personaC.PersonaDirecciones.First().FechaCreacion);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "FechaModificacion", personaC.PersonaDirecciones.First().FechaModificacion);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "UsuarioCreacion", personaC.PersonaDirecciones.First().UsuarioCreacion);
                                Database.SetParameterValue(comandoModificarPersonaCDireccion, "UsuarioModificacion", personaC.PersonaDirecciones.First().UsuarioModificacion);
                                Database.ExecuteNonQuery(comandoModificarPersonaCDireccion, transaction);

                                #endregion

                                #endregion

                                #region Contacto

                                Database.SetParameterValue(comandoModificarPersonaContacto, "IdPersonaContacto", personaC.PersonaContactos1.First().IdPersonaContacto);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "IdPersona", persona.IdPersona);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "IdContacto", personaC.IdPersona);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "Cargo", personaC.PersonaContactos1.First().Cargo);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "TipoContacto", personaC.PersonaContactos1.First().TipoContacto);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "FechaCreacion", personaC.PersonaContactos1.First().FechaCreacion);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "FechaModificacion", personaC.PersonaContactos1.First().FechaModificacion);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "UsuarioCreacion", personaC.PersonaContactos1.First().UsuarioCreacion);
                                Database.SetParameterValue(comandoModificarPersonaContacto, "UsuarioModificacion", personaC.PersonaContactos1.First().UsuarioModificacion);

                                Database.ExecuteNonQuery(comandoModificarPersonaContacto, transaction);

                                #endregion

                            }

                            #endregion
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                            throw new Exception(ex.InnerException.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }
            return true;
        }
        public IList<Persona> GetByFuncion(int idFuncion)
        {
            return Get("usp_Persona_GetByFuncion", idFuncion);
        }

        public IList<Persona> GetByFuncionFiltro(int idFuncion, string term)
        {
            return Get("usp_Persona_GetByFuncionFiltro", idFuncion, term);
        }
    }
}