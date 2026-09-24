using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IProveedorRepositoryFunctions
    {
        Proveedor? ObtenerPorSlug(string slug);

        bool ExisteNit(string nit, int? idProveedorExcluir = null);
    }
}