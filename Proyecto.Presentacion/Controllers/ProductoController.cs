using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Proyecto.Presentacion.Models;
using System.Text;

namespace Proyecto.Presentacion.Controllers
{
    public class ProductoController : Controller
    {
        //Cadena conexion hacia el servicio
        Uri baseAddress = new Uri("https://localhost:7108/api");
        private readonly HttpClient _httpClient;

        public ProductoController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        //PARA REPORTE
        [HttpGet]
        public IActionResult reporteProducto(string nombre = null, int? id_categoria = null, int? stock = null)
        {
            List<Producto> aProducto = new List<Producto>();

            // Construir la cadena de consulta dinámicamente
            var queryParameters = new List<string>();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryParameters.Add($"nombre={nombre}");
            }
            if (id_categoria.HasValue)
            {
                queryParameters.Add($"categoria={id_categoria.Value}");
            }
            if (stock.HasValue)
            {
                queryParameters.Add($"stock={stock.Value}");
            }
            var query = string.Join("&", queryParameters);

            // Si hay parámetros, añadir el prefijo "?"
            if (!string.IsNullOrEmpty(query))
            {
                query = "?" + query;
            }

            HttpResponseMessage response =
                _httpClient.GetAsync(_httpClient.BaseAddress + "/Producto/reporteProducto" + query).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aProducto = JsonConvert.DeserializeObject<List<Producto>>(data);
            }

            // Obtener la lista de categorías
            List<Categoria> categorias = aCategorias();

            // Pasar los valores actuales de los filtros a ViewBag
            ViewBag.CurrentFilterNombre = nombre;
            ViewBag.CurrentFilterCategoria = id_categoria;
            ViewBag.CurrentFilterStock = stock;


            // Pasar las categorías y la lista de productos a la vista
            ViewBag.categoria = new SelectList(categorias, "id_categoria", "nom_cat");
            return View(aProducto);
        }



        //FIN DE REPORTE 

        [HttpGet]
        public IActionResult listadoProductos()
        {
            List<Producto> aProductos = new List<Producto>();
            HttpResponseMessage response =
                _httpClient.GetAsync(_httpClient.BaseAddress + "/Producto/listadoProductos").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aProductos = JsonConvert.DeserializeObject<List<Producto>>(data);
            }
            return View(aProductos);
        }

        public List<Categoria> aCategorias()
        {
            List<Categoria> aCategorias = new List<Categoria>();
            HttpResponseMessage response =
            _httpClient.GetAsync(_httpClient.BaseAddress + "/Producto/listadoCategorias").Result;
            var data = response.Content.ReadAsStringAsync().Result;
            aCategorias = JsonConvert.DeserializeObject<List<Categoria>>(data);
            return aCategorias;
        }

        [HttpGet]
        public IActionResult nuevoProducto()
        {
            ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat");
            return View(new ProductoO());
        }
        [HttpPost]
        public async Task<IActionResult> nuevoProducto(ProductoO objP)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat");
                return View(objP);
            }
            var json = JsonConvert.SerializeObject(objP);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseC = await
            _httpClient.PostAsync("/api/Producto/nuevoProducto", content);
            if (responseC.IsSuccessStatusCode)
            {
                ViewBag.mensaje = "Producto registrado correctamente..!!!";
            }
            ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat");
            return View(objP);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
