namespace GymForce_API.Models
{
    public class Producto
    {
        public int id_producto { get; set; }
        public string? nom_prod { get; set; }
        public string? des_prod { get; set;}
        public string? nom_cat { get; set;}
        public double pre_prod { get; set; }
        public int stock {  get; set; }
        public string? raz_soc { get; set; }
    }
}
