
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;
    using System.Data;

    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public Usuario Insertar(Usuario entity)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Usuario

                        entity = Add(entity);

                        #endregion 

                        #region UsuarioSucursal

                        var usuarioSucursales = entity.UsuarioSucursal;
                        var comandoInsertarUsuarioSucursal = Database.GetStoredProcCommand("usp_Insert_UsuarioSucursal");
                       
                        Database.AddInParameter(comandoInsertarUsuarioSucursal, "IdEmpleado", DbType.Int32);
                        Database.AddInParameter(comandoInsertarUsuarioSucursal, "IdSucursal", DbType.Int32);
                        
                        foreach (var sucursal in usuarioSucursales)
                        {
                            Database.SetParameterValue(comandoInsertarUsuarioSucursal, "IdEmpleado", entity.IdEmpleado);
                            Database.SetParameterValue(comandoInsertarUsuarioSucursal, "IdSucursal", sucursal.IdSucursal);

                            Database.ExecuteNonQuery(comandoInsertarUsuarioSucursal, transaction);
                        }

                        #endregion Presentaciones

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

            //var permisoRoles = (from p in context.PermisoRol where p.IdRol == entity.IdRol select p);
            //foreach (PermisoRol permiso in permisoRoles)
            //{
            //    var actualPermiso = new PermisoUsuario
            //    {
            //        IdFormulario = permiso.IdFormulario,
            //        IdTipoPermiso = permiso.IdTipoPermiso,
            //        IdEmpleado = Convert.ToInt32(entity.IdEmpleado)
            //    };
            //    user.PermisoUsuario.Add(actualPermiso);
            //}
        }

        public Usuario Actualizar(Usuario entity)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Usuario

                        entity = Update(entity);

                        #endregion

                        #region UsuarioSucursal

                        var usuarioSucursales = entity.UsuarioSucursal;
                        var comandoEliminarUsuarioSucursal = Database.GetStoredProcCommand("usp_Delete_UsuarioSucursal");

                        Database.AddInParameter(comandoEliminarUsuarioSucursal, "IdEmpleado", DbType.Int32);
                        Database.SetParameterValue(comandoEliminarUsuarioSucursal, "IdEmpleado", entity.IdEmpleado);
                        Database.ExecuteNonQuery(comandoEliminarUsuarioSucursal, transaction);
                        

                        var comandoInsertarUsuarioSucursal = Database.GetStoredProcCommand("usp_Insert_UsuarioSucursal");

                        Database.AddInParameter(comandoInsertarUsuarioSucursal, "IdEmpleado", DbType.Int32);
                        Database.AddInParameter(comandoInsertarUsuarioSucursal, "IdSucursal", DbType.Int32);

                        foreach (var sucursal in usuarioSucursales)
                        {
                            Database.SetParameterValue(comandoInsertarUsuarioSucursal, "IdEmpleado", entity.IdEmpleado);
                            Database.SetParameterValue(comandoInsertarUsuarioSucursal, "IdSucursal", sucursal.IdSucursal);

                            Database.ExecuteNonQuery(comandoInsertarUsuarioSucursal, transaction);
                        }

                        #endregion Presentaciones

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

        public bool VerificarAccesoSucusal(string username, int idSucursal)
        {
            return Single("usp_Single_UsuarioSucursal", username, idSucursal) != null;
        }

        public IList<Usuario> GetUsersInRol(string roleName, string usernameToMatch)
        {
            return Get("usp_Select_Usuario_GetUsersInRol", roleName, usernameToMatch);
        }

        public IList<Usuario> GetUsersInRol(string roleName)
        {
            return Get("usp_Select_Usuario_GetUsersInRol", roleName,"");
        }

        public bool IsUserInRol(string username, string roleName)
        {
            return (int)GetScalar("usp_Select_Usuario_IsUserInRol", username, roleName) > 0;
        }

        public Usuario GetByCredenciales(string username, string password, int idEmpresa)
        {
            return Single("usp_Single_Usuario_ByCredendiales", username, password, idEmpresa);
        }
    }
}