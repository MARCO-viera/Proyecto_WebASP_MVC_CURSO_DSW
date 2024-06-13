using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proyecto.Presentacion.Models
{
    public class Proveedor
    {
        [DisplayName("CODIGO")]
        public int id_proveedor { get; set; }

        [DisplayName("RAZÓN SOCIAL")]
        [Required(ErrorMessage = "RAZÓN SOCIAL DEL PROVEEDOR")]
        public string? raz_soc { get; set; }

        [DisplayName("RUC")]
        [Required(ErrorMessage = "RUC DEL PROVEEDOR")]
        public string? ruc { get; set; }
    }
}
