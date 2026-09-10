using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace FERRETERIA__Joel.Pages
{
    public class ProductosModel : PageModel
    {
        private readonly IProductoRepository _productoRepository;

        public List<Producto> ListProductos { get; set; } = new();

        public ProductosModel(
            IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public void OnGet()
        {
            ListProductos =
                _productoRepository.ObtenerTodos();
        }
    }
}