
namespace JOERP.WebSite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class LoginModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [DisplayName("Usuario :")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contrseña es requerida.")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña :")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Por favor seleccione la Empresa.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione la Empresa.")]
        [DisplayName("Empresa :")]
        public int IdEmpresa { get; set; }

        [Required(ErrorMessage = "Por favor seleccione la Sucursal.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione la Sucursal.")]
        [DisplayName("Sucursal :")]
        public int IdSucursal { get; set; }

        public IEnumerable<SelectListItem> Empresas { get; set; }

        public IEnumerable<SelectListItem> Sucursales { get; set; }

        [DisplayName("Recordar mi cuenta?")]
        public bool RememberMe { get; set; }
    }
}