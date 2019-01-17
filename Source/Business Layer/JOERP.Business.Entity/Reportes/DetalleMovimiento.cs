using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JOERP.Business.Entity.Reportes
{
    public class DetalleMovimiento 
    {
        public int IdTransaccion { get; set; }
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public string Presentacion { get; set; }
        public string Lote { get; set; }
        public string Operacion { get; set; }
        public string Documento { get; set; }
        public string FechaRegistro { get; set; }
        public string Glosa { get; set; }
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public decimal Cantidad { get; set; }
    }
}
