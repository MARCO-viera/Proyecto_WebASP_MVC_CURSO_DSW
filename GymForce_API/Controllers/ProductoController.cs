using GymForce_API.Models;
using GymForce_API.Repositorio.DAO;
using Microsoft.AspNetCore.Mvc;

namespace GymForce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : Controller
    {
        [HttpGet("reporteProducto")]
        public async Task<ActionResult<List<Producto>>> reporteProducto(
           [FromQuery] string nombre = null,
           [FromQuery] int? categoria = null,
           [FromQuery] int? stock = null)
        {
            var lista = await Task.Run(() => new ProductoDAO().reporteProducto(nombre, categoria, stock));
            return Ok(lista);
        }

        [HttpGet("listadoProductos")]
        public async Task<ActionResult<List<Producto>>> listadoProducto()
        {
            var lista = await Task.Run(() => new ProductoDAO().listadoProducto());
            return Ok(lista);
        }
        [HttpGet("listadoProductoO")]
        public async Task<ActionResult<List<ProductoO>>> listadoProductoO()
        {
            var lista = await Task.Run(() => new ProductoDAO().listadoProductoO());
            return Ok(lista);
        }
        [HttpGet("listadoCategorias")]
        public async Task<ActionResult<List<Categoria>>> listadoCategorias()
        {
            var lista = await Task.Run(() =>
            new CategoriaDAO().listadoCategorias());
            return Ok(lista);
        }
        [HttpPost("nuevoProducto")]
        public async Task<ActionResult<string>> nuevoProducto(ProductoO objC)
        {
            var mensaje = await Task.Run(() =>
            new ProductoDAO().nuevoProducto(objC));
            return Ok(mensaje);
        }
        [HttpPut("modificaProducto")]
        public async Task<ActionResult<string>> modificaProducto(ProductoO objC)
        {
            var mensaje = await Task.Run(() =>
            new ProductoDAO().modificaProducto(objC));
            return Ok(mensaje);
        }
        [HttpGet("buscarProducto/{id}")]
        public async Task<ActionResult<List<ProductoO>>> buscarProducto(int id)
        {
            var lista = await Task.Run(() => new ProductoDAO().buscarProducto(id));
            return Ok(lista);
        }



        //PROVEEDOR
        [HttpGet("listadoProveedor")]
        public async Task<ActionResult<List<Proveedor>>> listadoProveedores()
        {
            var lista = await Task.Run(() =>
            new ProveedorDAO().listadoProveedores());
            return Ok(lista);
        }
        [HttpPost("nuevoProveedor")]
        public async Task<ActionResult<string>> nuevoProveedor(Proveedor objC)
        {
            var mensaje = await Task.Run(() =>
            new ProveedorDAO().nuevoProveedor(objC));
            return Ok(mensaje);
        }
        [HttpPut("modificaProveedor")]
        public async Task<ActionResult<string>> modificaProveedor(Proveedor objC)
        {
            var mensaje = await Task.Run(() =>
            new ProveedorDAO().modificaProveedor(objC));
            return Ok(mensaje);
        }
        [HttpGet("buscarProveedor/{id}")]
        public async Task<ActionResult<List<Proveedor>>> buscarProveedor(int id)
        {
            var lista = await Task.Run(() => new ProveedorDAO().buscarProveedor(id));
            return Ok(lista);
        }
    }
}