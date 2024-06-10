﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult GenerarExcel(string nombre = null, int? id_categoria = null, int? stock = null)
        {
            // Obtener los datos filtrados
            List<Producto> productosFiltrados = ObtenerProductosFiltrados(nombre, id_categoria, stock);

            // Crear el libro de Excel y la hoja de trabajo
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Productos");

            // Agregar encabezados de columna
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Descripción";
            worksheet.Cell(1, 4).Value = "Categoría";
            worksheet.Cell(1, 5).Value = "Precio";
            worksheet.Cell(1, 6).Value = "Stock";

            // Aplicar estilos a los encabezados
            var headersRow = worksheet.Row(1);
            headersRow.Style.Font.Bold = true; // Texto en negrita
            headersRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Alineación horizontal centrada
            headersRow.Style.Fill.BackgroundColor = XLColor.LightBlue; // Color de fondo celeste

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

                // Aplicar bordes a las celdas de datos
                row.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde delgado en el exterior de la celda
                                                                           // Puedes agregar más estilos de bordes según tus necesidades
            }

            // Autoajustar ancho de las columnas
            worksheet.Columns().AdjustToContents();

            // Guardar el libro de Excel en un MemoryStream
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                // Devolver el archivo Excel como una descarga
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos.xlsx");
            }
        }

            private List<Producto> ObtenerProductosFiltrados(string nombre, int? id_categoria, int? stock)
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
