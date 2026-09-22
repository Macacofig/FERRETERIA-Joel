using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface ICategoriaRepositoryFunctions
    {
        List<Categoria> ObtenerActivas();

        void Desactivar(int id);

        string ObtenerSiguienteCodigo();

        bool ExisteCodigo(string codigo, int? idCategoriaExcluir = null);
    }
}