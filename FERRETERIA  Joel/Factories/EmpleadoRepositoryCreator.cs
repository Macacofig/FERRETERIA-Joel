using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class EmpleadoRepositoryCreator : RepositoryCreator<IRepository<Empleado>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmpleadoRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IRepository<Empleado> CreateRepository()
        {
            return new MySqlEmpleadoRepository(_connectionFactory);
        }
    }
}