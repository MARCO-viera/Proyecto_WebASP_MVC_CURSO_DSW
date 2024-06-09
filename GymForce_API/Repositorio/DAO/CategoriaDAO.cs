using GymForce_API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GymForce_API.Repositorio.DAO
{
    public class CategoriaDAO
    {
        //DEFINIR LA CADENA DE CONEXION
        private readonly string? cadena;

        public CategoriaDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("cn");
        }

        public IEnumerable<Categoria> listadoCategorias()
        {
            List<Categoria> aCategorias = new List<Categoria>();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            SqlCommand cmd = new SqlCommand("SP_LISTADOCATEGORIAS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                aCategorias.Add(new Categoria
                {
                    id_categoria = int.Parse(dr[0].ToString()),
                    nom_cat = dr[1].ToString()
                });
            }
            cn.Close();
            return aCategorias;
        }
    }
}
