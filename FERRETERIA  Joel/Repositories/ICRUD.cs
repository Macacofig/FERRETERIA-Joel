namespace FERRETERIA__Joel.Repositories
{
    public interface ICRUD<T>
    {
        List<T> ObtenerTodos();

        T? ObtenerPorId(int id);

        int Insertar(T entidad);

        void Actualizar(T entidad);

        void CambiarEstado(int id);
    }
}