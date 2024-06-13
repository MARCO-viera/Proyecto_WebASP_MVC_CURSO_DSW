using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Proyecto.Presentacion.Models;
using System.Text;
//PARA EXCEL
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;

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
        public IActionResult reporteProducto(string nombre = null, int? id_categoria = null, int? stock = null, int? id_proveedor = null)
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
            if (id_proveedor.HasValue)
            {
                queryParameters.Add($"proveedor={id_proveedor.Value}");
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
            ViewBag.CurrentFilterProveedor = id_proveedor;


            // Pasar las categorías y la lista de productos a la vista
            ViewBag.categoria = new SelectList(categorias, "id_categoria", "nom_cat");
            ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc");
            return View(aProducto);
        }
        [HttpPost]
        public IActionResult GenerarExcel(string nombre = null, int? id_categoria = null, int? stock = null, int? id_proveedor = null)
        {
            // Obtener los datos filtrados
            List<Producto> productosFiltrados = ObtenerProductosFiltrados(nombre, id_categoria, stock, id_proveedor);

            // Crear el libro de Excel y la hoja de trabajo
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Productos");

            // Agregar encabezados de columna con estilo ejecutivo
            var headersRow = worksheet.Row(1);
            headersRow.Style.Font.Bold = true; // Texto en negrita
            headersRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Alineación horizontal centrada
            headersRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#337ab7"); // Color de fondo azul ejecutivo
            headersRow.Style.Font.FontColor = XLColor.White; // Color de fuente blanco


            // Agregar nombres de encabezados
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Descripción";
            worksheet.Cell(1, 4).Value = "Categoría";
            worksheet.Cell(1, 5).Value = "Precio";
            worksheet.Cell(1, 6).Value = "Stock";
            worksheet.Cell(1, 7).Value = "Proveedor";

            // Ajustar el ancho de las columnas
            for (int i = 1; i <= 8; i++)
            {
                worksheet.Column(i).AdjustToContents();
            }

            // Agregar datos filtrados al archivo Excel
            for (int i = 0; i < productosFiltrados.Count; i++)
            {
                var row = worksheet.Row(i + 2); // Comienza desde la segunda fila (encabezados en la primera fila)

                row.Cell(1).Value = productosFiltrados[i].id_producto;
                row.Cell(2).Value = productosFiltrados[i].nom_prod;
                row.Cell(3).Value = productosFiltrados[i].des_prod;
                row.Cell(4).Value = productosFiltrados[i].nom_cat;
                row.Cell(5).Value = productosFiltrados[i].pre_prod;
                row.Cell(6).Value = productosFiltrados[i].stock;
                row.Cell(7).Value = productosFiltrados[i].raz_soc;

                // Aplicar estilos a las celdas de datos
                row.Style.Fill.BackgroundColor = XLColor.FromHtml("#f7f7f7"); // Color de fondo gris claro
                row.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde delgado en el exterior de la celda
                row.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde delgado dentro de la celda
                row.Style.Font.FontName = "Arial"; // Fuente Arial
            }

            // Guardar el libro de Excel en un MemoryStream
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                // Devolver el archivo Excel como una descarga
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos.xlsx");
            }
        }

        private List<Producto> ObtenerProductosFiltrados(string nombre, int? id_categoria, int? stock, int? id_proveedor)
        {
            // Construir la cadena de consulta dinámicamente
            var queryParameters = new List<string>();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryParameters.Add($"nombre={nombre}");
            }
            if (id_categoria.HasValue)
            {
                queryParameters.Add($"id_categoria={id_categoria.Value}");
            }
            if (stock.HasValue)
            {
                queryParameters.Add($"stock={stock.Value}");
            }
            if (id_proveedor.HasValue)
            {
                queryParameters.Add($"proveedor={id_proveedor.Value}");
            }
            var query = string.Join("&", queryParameters);

            // Si hay parámetros, añadir el prefijo "?"
            if (!string.IsNullOrEmpty(query))
            {
                query = "?" + query;
            }

            // Realizar la llamada a la API para obtener los datos filtrados
            HttpResponseMessage response =
                _httpClient.GetAsync(_httpClient.BaseAddress + "/Producto/reporteProducto" + query).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<Producto>>(data);
            }

            // En caso de error o si no se obtienen datos, devolver una lista vacía o manejar el error según sea necesario
            return new List<Producto>();
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
            ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc");
            return View(new ProductoO());
        }

        [HttpPost]
        public async Task<IActionResult> nuevoProducto(ProductoO objP)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat");
                ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc");
                return View(objP);
            }

            var json = JsonConvert.SerializeObject(objP);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseC = await _httpClient.PostAsync("/api/Producto/nuevoProducto", content);

            if (responseC.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Producto registrado correctamente..!!!";
                return RedirectToAction(nameof(nuevoProducto));
            }
            else
            {
                TempData["ErrorMessage"] = "Error en el registro del producto.";
            }

            ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat");
            ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc");
            return View(objP);
        }



        [HttpGet]
        public async Task<IActionResult> modificarProducto(int id)
        {
            // Obtener detalles del producto desde la API
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Producto/buscarProducto/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var producto = JsonConvert.DeserializeObject<ProductoO>(data);
                ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat", producto.id_categoria);
                ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc", producto.id_proveedor);
                return View(producto);
            }
            else
            {
                // Manejar el error de búsqueda del producto
                TempData["ErrorMessage"] = "No se pudo encontrar el producto para modificar.";
                return RedirectToAction(nameof(listadoProductos));
            }
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> modificarProducto(ProductoO objP)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat", objP.id_categoria);
                ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc", objP.id_proveedor);
                return View(objP);
            }

            var json = JsonConvert.SerializeObject(objP);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Producto/modificaProducto", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Producto actualizado correctamente..!!!";
                return RedirectToAction(nameof(listadoProductos));
            }
            else
            {
                TempData["ErrorMessage"] = "Error al actualizar el producto.";
            }

            ViewBag.categoria = new SelectList(aCategorias(), "id_categoria", "nom_cat", objP.id_categoria);
            ViewBag.proveedor = new SelectList(aProveedores(), "id_proveedor", "raz_soc", objP.id_proveedor);
            return View(objP);
        }








        //PROVEEDOR
        public List<Proveedor> aProveedores()
        {
            List<Proveedor> aProveedores = new List<Proveedor>();
            HttpResponseMessage response =
            _httpClient.GetAsync(_httpClient.BaseAddress + "/Producto/listadoProveedor").Result;
            var data = response.Content.ReadAsStringAsync().Result;
            aProveedores = JsonConvert.DeserializeObject<List<Proveedor>>(data);
            return aProveedores;
        }

        public IActionResult listadoProveedor()
        {
            List<Proveedor> aProveedores = new List<Proveedor>();
            HttpResponseMessage response =
            _httpClient.GetAsync(_httpClient.BaseAddress + "/Producto/listadoProveedor").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aProveedores = JsonConvert.DeserializeObject<List<Proveedor>>(data);
            }
            return View(aProveedores);
        }

        [HttpGet]
        public IActionResult nuevoProveedor()
        {
            return View(new Proveedor());
        }

        [HttpPost]
        public async Task<IActionResult> nuevoProveedor(Proveedor objPv)
        {
            if (!ModelState.IsValid)
            {
                return View(objPv);
            }

            var json = JsonConvert.SerializeObject(objPv);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseC = await _httpClient.PostAsync("/api/Producto/nuevoProveedor", content);

            if (responseC.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Proveedor registrado correctamente..!!!";
                return RedirectToAction(nameof(nuevoProveedor));
            }
            else
            {
                TempData["ErrorMessage"] = "Error en el registro del proveedor.";
            }
            return View(objPv);
        }



        [HttpGet]
        public async Task<IActionResult> modificarProveedor(int id)
        {
            // Obtener detalles del producto desde la API
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Producto/buscarProveedor/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var proveedor = JsonConvert.DeserializeObject<Proveedor>(data);
                return View(proveedor);
            }
            else
            {
                // Manejar el error de búsqueda del producto
                TempData["ErrorMessage"] = "No se pudo encontrar el proveedor para modificar.";
                return RedirectToAction(nameof(listadoProductos));
            }
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> modificarProveedor(Proveedor objPv)
        {
            if (!ModelState.IsValid)
            {
                return View(objPv);
            }

            var json = JsonConvert.SerializeObject(objPv);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Producto/modificaProveedor", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Proveedor actualizado correctamente..!!!";
                return RedirectToAction(nameof(listadoProveedor));
            }
            else
            {
                TempData["ErrorMessage"] = "Error al actualizar el proveedor.";
            }
            return View(objPv);
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}