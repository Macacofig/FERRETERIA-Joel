using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    // Empleado no tiene funciones especiales, así que su Producto
    // sigue siendo el ICRUD<Empleado> genérico (sin interfaz combinada).
    public class EmpleadoRepositoryCreator : RepositoryCreator<ICRUD<Empleado>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmpleadoRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override ICRUD<Empleado> CreateRepository()
        {
            return new MySqlEmpleadoRepository(_connectionFactory);
        }
    }
}
