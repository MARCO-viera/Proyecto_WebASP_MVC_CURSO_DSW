using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proyecto.Presentacion.Models
{
    public class ProductoO
    {
        [DisplayName("CODIGO")]
        public int id_producto { get; set; }

        [Required(ErrorMessage = "NOMBRE DEL PRODUCTO")]
        [DisplayName("PRODUCTO")]
        public string? nom_prod { get; set; }

        [Required(ErrorMessage = "DESCRIPCIÓN DEL PRODUCTO")]
        [DisplayName("DESCRIPCIÓN")]
        public string? des_prod { get; set; }

        [Required(ErrorMessage = "CATEGORÍA DEL PRODUCTO")]
        [DisplayName("CATEGORÍA")]
        public int id_categoria { get; set; }

        [Required(ErrorMessage = "PRECIO DEL PRODUCTO")]
        [DisplayName("PRECIO")]
        public double pre_prod { get; set; }

        [Required(ErrorMessage = "STOCK DEL PRODUCTO")]
        [DisplayName("STOCK")]
        public int stock { get; set; }
    }
}
