namespace FERRETERIA__Joel.Repositories
{
    public interface IProveedorRepositoryFunctions
    {
        bool ExisteNit(string nit, int? idProveedorExcluir = null);
    }
}