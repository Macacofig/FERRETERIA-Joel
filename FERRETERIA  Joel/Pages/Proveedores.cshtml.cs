using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedoresModel : PageModel
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ILogger<ProveedoresModel> _logger;

        public List<Proveedor> ListProveedores { get; set; } = new();

        public ProveedoresModel(
            RepositoryCreator<IProveedorRepository> proveedorRepositoryCreator,
            ILogger<ProveedoresModel> logger)
        {
            _proveedorRepository = proveedorRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                ListProveedores = _proveedorRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al cargar el listado de proveedores.");

                TempData["MensajeError"] =
                    "No se pudo cargar el listado de proveedores.";
            }
        }

        public IActionResult OnPostEliminar(int idProveedor)
        {
            try
            {
                _proveedorRepository.CambiarEstado(idProveedor);

                TempData["Mensaje"] =
                    "Proveedor eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al eliminar el proveedor {IdProveedor}.",
                    idProveedor);

                TempData["MensajeError"] =
                    "No se pudo eliminar el proveedor. Inténtalo nuevamente.";
            }

            return RedirectToPage();
        }
    }
}