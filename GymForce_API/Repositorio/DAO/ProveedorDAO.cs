using GymForce_API.Models;
using GymForce_API.Repositorio.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GymForce_API.Repositorio.DAO
{
    public class ProveedorDAO
    {
        //DEFINIR LA CADENA DE CONEXION
        private readonly string? cadena;

        public ProveedorDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("cn");
        }

        public IEnumerable<Proveedor> listadoProveedores()
        {
            List<Proveedor> aProveedores = new List<Proveedor>();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            SqlCommand cmd = new SqlCommand("SP_LISTADOPROVEEDORES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                aProveedores.Add(new Proveedor
                {
                    id_proveedor = int.Parse(dr[0].ToString()),
                    raz_soc = dr[1].ToString(),
                    ruc = dr[2].ToString()
                });
            }
            cn.Close();
            return aProveedores;
        }


        public Proveedor buscarProveedor(int id)
        {
            Proveedor proveedor = null;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ID_PROVEEDOR, RAZ_SOC, RUC FROM PROVEEDOR WHERE ID_PROVEEDOR = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            proveedor = new Proveedor
                            {
                                id_proveedor = dr.GetInt32(dr.GetOrdinal("ID_PROVEEDOR")),
                                raz_soc = dr.GetString(dr.GetOrdinal("RAZ_SOC")),
                                ruc = dr.GetString(dr.GetOrdinal("RUC"))
                            };
                        }
                    }
                }
            }
            return proveedor;
        }


        public string modificaProveedor(Proveedor objPv)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_MERGE_PROVEEDOR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ide", objPv.id_proveedor);
                    cmd.Parameters.AddWithValue("@raz_soc", objPv.raz_soc);
                    cmd.Parameters.AddWithValue("@ruc", objPv.ruc);
                    int n = cmd.ExecuteNonQuery();
                    mensaje = n.ToString() + " Proveedor actualizado...!!!";
                }
                catch (Exception ex)
                {
                    mensaje = "Error al actualizar...!! " + ex.Message;
                }
            }
            return mensaje;
        }



        public string nuevoProveedor(Proveedor objPv)
        {
            string mensaje = "";
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_MERGE_PROVEEDOR", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ide", objPv.id_proveedor);
                cmd.Parameters.AddWithValue("@raz_soc", objPv.raz_soc);
                cmd.Parameters.AddWithValue("@ruc", objPv.ruc);
                int n = cmd.ExecuteNonQuery();
                mensaje = n.ToString() + "Proveedor registrado...!!!";
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar..!!" + ex.Message;
            }
            cn.Close();
            return mensaje;
        }
    }
}

