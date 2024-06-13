namespace GymForce_API.Models
{
    public class ProductoO
    {
        public int id_producto { get; set; }
        public string? nom_prod { get; set; }
        public string? des_prod { get; set; }
        public int id_categoria { get; set; }
        public double pre_prod { get; set; }
        public int stock { get; set; }
        public int id_proveedor { get; set; }
    }
}
