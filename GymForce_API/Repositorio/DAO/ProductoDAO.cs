using GymForce_API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Numerics;

namespace GymForce_API.Repositorio.DAO
{
    public class ProductoDAO
    {
        //DEFINIR LA CADENA DE CONEXION
        private readonly string? cadena;

        public ProductoDAO()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("cn");
        }
        public ProductoO buscarProducto(int id)
        {
            ProductoO producto = null;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ID_PRODUCTO, NOM_PROD, DES_PROD, ID_CATEGORIA, PRE_PROD, STOCK,ID_PROVEEDOR FROM PRODUCTOS WHERE ID_PRODUCTO = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            producto = new ProductoO
                            {
                                id_producto = dr.GetInt32(dr.GetOrdinal("ID_PRODUCTO")),
                                nom_prod = dr.GetString(dr.GetOrdinal("NOM_PROD")),
                                des_prod = dr.GetString(dr.GetOrdinal("DES_PROD")),
                                id_categoria = dr.GetInt32(dr.GetOrdinal("ID_CATEGORIA")),
                                pre_prod = Convert.ToDouble(dr.GetDecimal(dr.GetOrdinal("PRE_PROD"))),
                                stock = dr.GetInt32(dr.GetOrdinal("STOCK")),
                                id_proveedor = dr.GetInt32(dr.GetOrdinal("ID_PROVEEDOR"))
                            };
                        }
                    }
                }
            }
            return producto;
        }



        //PARA REPORTE
        public IEnumerable<Producto> reporteProducto(string nombre = null, int? categoria = null, int? stock = null, int? proveedor = null)
        {
            List<Producto> aProducto = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SP_REPORTEPRODUCTOS", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Añadir parámetros con valores nulos si no se proporcionan
                    cmd.Parameters.AddWithValue("@NOMBRE", (object)nombre ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CATEGORIA", (object)categoria ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@STOCK", (object)stock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PROVEEDOR", (object)proveedor ?? DBNull.Value);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            aProducto.Add(new Producto
                            {
                                id_producto = dr.GetInt32(dr.GetOrdinal("ID_PRODUCTO")),
                                nom_prod = dr.GetString(dr.GetOrdinal("NOM_PROD")),
                                des_prod = dr.GetString(dr.GetOrdinal("DES_PROD")),
                                nom_cat = dr.GetString(dr.GetOrdinal("NOM_CAT")),
                                pre_prod = Convert.ToDouble(dr.GetDecimal(dr.GetOrdinal("PRE_PROD"))), 
                                stock = dr.GetInt32(dr.GetOrdinal("STOCK")),
                                raz_soc = dr.GetString(dr.GetOrdinal("RAZ_SOC")),
                            });
                        }
                    }
                }
            }
            return aProducto;
        }

    //FIN DE REPORTE
    public IEnumerable<Producto> listadoProducto()
        {
            List<Producto> aProductos = new List<Producto>();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            SqlCommand cmd = new SqlCommand("SP_LISTADOPRODUCTOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                aProductos.Add(new Producto
                {
                    id_producto = int.Parse(dr[0].ToString()),
                    nom_prod = dr[1].ToString(),
                    des_prod = dr[2].ToString(),
                    nom_cat = dr[3].ToString(),
                    pre_prod = double.Parse(dr[4].ToString()),
                    stock = int.Parse(dr[5].ToString()),
                    raz_soc = dr[6].ToString(),
                });
            }
            cn.Close();
            return aProductos;
        }

        public IEnumerable<ProductoO> listadoProductoO()
        {
            List<ProductoO> aProductos = new List<ProductoO>();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            SqlCommand cmd = new SqlCommand("SP_LISTADOPRODUCTOS_O", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                aProductos.Add(new ProductoO
                {
                    id_producto = int.Parse(dr[0].ToString()),
                    nom_prod = dr[1].ToString(),
                    des_prod = dr[2].ToString(),
                    id_categoria = int.Parse(dr[3].ToString()),
                    pre_prod = double.Parse(dr[4].ToString()),
                    stock = int.Parse(dr[5].ToString()),
                    id_proveedor = int.Parse(dr[6].ToString()),
                });
            }
            cn.Close();
            return aProductos;
        }

        public string modificaProducto(ProductoO objP)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_MERGE_PRODUCTO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ide", objP.id_producto);
                    cmd.Parameters.AddWithValue("@nom", objP.nom_prod);
                    cmd.Parameters.AddWithValue("@des", objP.des_prod);
                    cmd.Parameters.AddWithValue("@cat", objP.id_categoria);
                    cmd.Parameters.AddWithValue("@pre", objP.pre_prod);
                    cmd.Parameters.AddWithValue("@stock", objP.stock);
                    cmd.Parameters.AddWithValue("@prov", objP.id_proveedor);
                    int n = cmd.ExecuteNonQuery();
                    mensaje = n.ToString() + " Producto actualizado...!!!";
                }
                catch (Exception ex)
                {
                    mensaje = "Error al actualizar...!! " + ex.Message;
                }
            }
            return mensaje;
        }



        public string nuevoProducto(ProductoO objP)
        {
            string mensaje = "";
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_MERGE_PRODUCTO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ide", objP.id_producto);
                cmd.Parameters.AddWithValue("@nom", objP.nom_prod);
                cmd.Parameters.AddWithValue("@des", objP.des_prod);
                cmd.Parameters.AddWithValue("@cat", objP.id_categoria);
                cmd.Parameters.AddWithValue("@pre", objP.pre_prod);
                cmd.Parameters.AddWithValue("@stock", objP.stock);
                cmd.Parameters.AddWithValue("@prov", objP.id_proveedor);
                int n = cmd.ExecuteNonQuery();
                mensaje = n.ToString() + "Producto registrado...!!!";
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


