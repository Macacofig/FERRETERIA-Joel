using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    // Producto del Factory Method: combina el CRUD genérico con las
    // funciones especiales de HistoricoPrecio en un único contrato.
    public interface IHistoricoPrecioRepository : ICRUD<HistoricoPrecio>, IHistoricoPrecioRepositoryFunctions
    {
    }
}
