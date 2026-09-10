using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public interface IEmpleadoRepository
    {
        List<Empleado> ObtenerActivos();
    }
}
