using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    // Producto del Factory Method: combina el CRUD genérico con las
    // funciones especiales de Producto en un único contrato.
    public interface IProductoRepository : ICRUD<Producto>, IProductoRepositoryFunctions
    {
    }
}
