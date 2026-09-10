using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IProveedorRepository
    {
        List<Proveedor> ObtenerTodos();

        Proveedor? ObtenerPorId(short idProveedor);

        void Insertar(Proveedor proveedor);

        void Actualizar(Proveedor proveedor);

        void CambiarEstado(short idProveedor);

        bool ExisteNit(string nit, short? idProveedorExcluir = null);
    }
}
