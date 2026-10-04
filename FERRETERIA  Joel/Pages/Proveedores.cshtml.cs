using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedoresModel : PageModel
    {
        private readonly IRepository<Proveedor> _proveedorRepository;
        private readonly IModificacionRepository<Proveedor> _proveedorModificacionRepository;
        private readonly ILogger<ProveedoresModel> _logger;

        public List<Proveedor> ListProveedores { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public bool SoloActivos { get; set; }

        public ProveedoresModel(
            RepositoryCreator<IRepository<Proveedor>> proveedorRepositoryCreator,
            IModificacionRepository<Proveedor> modificacionRepository,
            ILogger<ProveedoresModel> logger)
        {
            _proveedorRepository = proveedorRepositoryCreator.CreateRepository();
            _proveedorModificacionRepository = modificacionRepository;
            _logger = logger;
        }

        public void OnGet(bool? soloActivos)
        {
            SoloActivos = soloActivos ?? false;

            try
            {
                ListProveedores = SoloActivos
                    ? _proveedorRepository.ObtenerActivas()
                    : _proveedorRepository.ObtenerTodos();
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
                _proveedorModificacionRepository.CambiarEstado(idProveedor);

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

            return RedirectToPage(new { soloActivos = SoloActivos });
        }
    }
}