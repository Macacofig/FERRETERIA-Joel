namespace FERRETERIA__Joel.Repositories
{
    public interface IRepository<T>
    {
        List<T> ObtenerTodos();

        List<T> ObtenerActivas();

        T? ObtenerPorId(int id);

        int Insertar(T entidad);
    }
}
