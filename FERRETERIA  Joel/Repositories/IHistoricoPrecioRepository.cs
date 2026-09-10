using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IHistoricoPrecioRepository
    {
        List<HistoricoPrecio> ObtenerPorProducto(int idProducto);
        void Insertar(HistoricoPrecio historicoPrecio);
        HistoricoPrecio? ObtenerPrecioVigente(int idProducto);
        void CerrarPrecioVigente(int idProducto);
    }
}
