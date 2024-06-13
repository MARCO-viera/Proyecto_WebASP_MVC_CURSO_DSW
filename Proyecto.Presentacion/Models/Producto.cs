using System.ComponentModel;

namespace Proyecto.Presentacion.Models
{
    public class Producto
    {
        [DisplayName("CODIGO")]
        public int id_producto { get; set; }

        [DisplayName("PRODUCTO")]
        public string? nom_prod { get; set; }

        [DisplayName("DESCRIPCIÓN")]
        public string? des_prod { get; set; }
    
        [DisplayName("CATEGORÍA")]
        public string? nom_cat { get; set; }

        [DisplayName("PRECIO")]
        public double pre_prod { get; set; }

        [DisplayName("STOCK")]
        public int stock { get; set; }

        [DisplayName("PROVEEDOR")]
        public string? raz_soc { get; set; }
    }
}
