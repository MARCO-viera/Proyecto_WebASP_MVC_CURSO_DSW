using GymForce_API.Models;

namespace GymForce_API.Repositorio.Interfaces
{
    public interface ICategoria
    {
        //Llenar el combo en la presentacion
        IEnumerable<Categoria> listadoCatgeorias();
    }
}
