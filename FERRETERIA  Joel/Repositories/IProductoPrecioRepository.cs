using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IProductoPrecioRepository
    {
        void ActualizarConHistorico(Producto producto);

        int InsertarConHistorico(Producto producto);
    }
}