using GymForce_API.Models;

namespace GymForce_API.Repositorio.Interfaces
{
    public interface IProducto
    {
        IEnumerable<Producto> listadoProducto();
        IEnumerable<ProductoO> listadoProductoO();
        ProductoO buscarProducto(int id);
        string nuevoProducto(ProductoO objP);
        string modificaProducto(ProductoO objP);
    }
}
