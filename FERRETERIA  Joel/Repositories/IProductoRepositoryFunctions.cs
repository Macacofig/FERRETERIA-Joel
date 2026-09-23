using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IProductoRepositoryFunctions
    {
        Producto? ObtenerPorSlug(string slug);

        bool ExisteCodigo(string codigo, int? idProductoExcluir = null);

        bool ExisteCategoriaActiva(int idCategoria);

        string ObtenerSiguienteCodigo();
    }
}