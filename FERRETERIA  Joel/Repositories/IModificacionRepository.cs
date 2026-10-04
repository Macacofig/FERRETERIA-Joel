namespace FERRETERIA__Joel.Repositories
{
    public interface IModificacionRepository<T>
    {
        void Actualizar(T entidad);

        void CambiarEstado(int id);

        int Count();
    }
}
