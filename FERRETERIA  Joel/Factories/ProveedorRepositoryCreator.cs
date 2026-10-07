using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class ProveedorRepositoryCreator : RepositoryCreator<IRepository<Proveedor>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProveedorRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IRepository<Proveedor> CreateRepository()
        {
            return new MySqlProveedorRepository(_connectionFactory);
        }
    }
}