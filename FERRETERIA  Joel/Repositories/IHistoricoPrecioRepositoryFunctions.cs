using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IHistoricoPrecioRepositoryFunctions
    {
        List<HistoricoPrecio> ObtenerPorProducto(int idProducto);

        HistoricoPrecio? ObtenerPrecioVigente(int idProducto);

        void CerrarPrecioVigente(int idProducto);
    }
}