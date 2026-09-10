namespace FERRETERIA__Joel.Repositories
{
    public interface IProductoRepository
    {
        List<Models.Producto> ObtenerTodos();

        Models.Producto? ObtenerPorId(int idProducto);

        int Insertar(Models.Producto producto);

        void Actualizar(Models.Producto producto);

        void CambiarEstado(int idProducto);

        bool ExisteCodigo(string codigo, int? idProductoExcluir = null);

        bool ExisteCategoriaActiva(short idCategoria);
    }
}
