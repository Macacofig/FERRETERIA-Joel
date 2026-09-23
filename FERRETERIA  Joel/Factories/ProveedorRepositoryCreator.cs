using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class ProveedorRepositoryCreator : RepositoryCreator<Proveedor>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProveedorRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override ICRUD<Proveedor> CreateRepository()
        {
            return new MySqlProveedorRepository(_connectionFactory);
        }
    }
}