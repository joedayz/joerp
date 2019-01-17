using System;
using JOERP.Business.Entity;
using JOERP.Business.Logic;
using JOERP.Helpers.Enums;

namespace JOERP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var formulario = new Form1();
            formulario.ShowDialog();

            /*var cantidad = EmpresaBL.Instancia.Count();
            System.Console.WriteLine(cantidad);

            var empresas = EmpresaBL.Instancia.Get();
            foreach (var empresa in empresas)
            {
                System.Console.WriteLine(empresa.RazonSocial);
            }

            var empresaNueva = new Empresa
                                   {
                                       RazonSocial = "Demo",
                                       RUC = "12345678901",
                                       Direccion = "Av. 123 Lima",
                                       Estado = (int)TipoEstado.Activo,
                                       CodigoPostal = "Lima 01",
                                       IdUbigeo = 1,
                                       ActividadEconomica = 1,
                                       Telefono = "(01) 123456",
                                       Celular = "123456789",
                                       Fax = "no tengo",
                                       PaginaWeb = "not fond",
                                       CorreoElectronico = "empresa@123.net",
                                       TipoContribuyente = 1,
                                       FechaInscripcion = DateTime.Now.AddDays(1),
                                       FechaActividades = DateTime.Now.AddDays(2),
                                       FechaCreacion = DateTime.Now,
                                       FechaModificacion = DateTime.Now,
                                       UsuarioCreacion = 1,
                                       UsuarioModificacion = 1,
                                       EsPercepcion = true
                                   };

            EmpresaBL.Instancia.Add(empresaNueva);

            System.Console.WriteLine("-------");
            empresas = EmpresaBL.Instancia.Get();
            foreach (var empresa in empresas)
            {
                System.Console.WriteLine(empresa.RazonSocial);
            }

            var empresaModificar = EmpresaBL.Instancia.Single(empresaNueva.IdEmpresa);

            empresaModificar.RazonSocial = "Demo 123 SAC";
            empresaModificar.FechaModificacion = DateTime.Now;
            empresaModificar.UsuarioModificacion = 2;

            EmpresaBL.Instancia.Update(empresaModificar);

            System.Console.WriteLine("-------");
            empresas = EmpresaBL.Instancia.Get();
            foreach (var empresa in empresas)
            {
                System.Console.WriteLine(empresa.RazonSocial);
            }

            System.Console.WriteLine("-------");

            var empresaBuscado = EmpresaBL.Instancia.SingleByRazonSocial("JOEDAYZ EIRL");
            if (empresaBuscado != null)
            {
                System.Console.WriteLine("Encontrado " + empresaBuscado.RazonSocial);   
            }

            EmpresaBL.Instancia.Delete(empresaNueva.IdEmpresa);

            System.Console.WriteLine("-------");
            empresas = EmpresaBL.Instancia.Get();
            foreach (var empresa in empresas)
            {
                System.Console.WriteLine(empresa.RazonSocial);
            }

            System.Console.ReadKey();*/
        }
         
    }
}
