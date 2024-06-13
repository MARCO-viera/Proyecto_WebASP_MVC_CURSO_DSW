using GymForce_API.Models;

namespace GymForce_API.Repositorio.Interfaces
{
    public interface IProveedor
    {
        //Llenar el combo en la presentacion
        IEnumerable<Proveedor> listadoProveedores();
        Proveedor buscarProveedor(int id);
        string nuevoProveedor(Proveedor objPv);
        string modificaProveedor(Proveedor objPv);
    }
}
