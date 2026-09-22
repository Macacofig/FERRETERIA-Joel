namespace FERRETERIA__Joel.Repositories
{
    public interface IProductoRepositoryFunctions
    {
        bool ExisteCodigo(string codigo, int? idProductoExcluir = null);

        bool ExisteCategoriaActiva(int idCategoria);

        string ObtenerSiguienteCodigo();
    }
}